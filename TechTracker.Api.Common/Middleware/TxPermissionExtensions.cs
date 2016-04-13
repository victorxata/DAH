using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using MongoDB.Driver;
using Owin;
using TechTracker.Common.Tracer;
using TechTracker.Common.Utils.Configuration;
using TechTracker.Domain.Data.Identity;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.MongoDb;
using TechTracker.Services;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Common.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public static class RbacPermissionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppBuilder UseRbacPermissions(this IAppBuilder app)
        {
            
            app.Use(typeof (RbacPermissionsMiddleware));
            return app;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RbacPermissionOptions
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class RbacPermissionsMiddleware : OwinMiddleware
    {
        private readonly IRolesService _rolesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RbacPermissionsMiddleware(OwinMiddleware next)
            : base(next)
        {
            var logger = new LoggerWrapper(new AppConfigManager());
            _rolesService = new RolesService(new RolesRepository(), new PermissionsRepository(), new PermissionsService(new PermissionsRepository(), logger), logger );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            var method = context.Request.Method;

            // get the endpoint and user identity from the owin context
            var userIdentity = context.Authentication.User;

            if (userIdentity != null)
            {
                var userIsAuthenticated = userIdentity.Identity.IsAuthenticated;

                if (userIsAuthenticated)
                {
                    var appUser = await Managers.UsersCollection
                        .Find(Builders<ApplicationUser>.Filter.Eq(x => x.UserName, userIdentity.Identity.Name))
                        .FirstOrDefaultAsync();


                    var roles = await _rolesService.GetAsync();
                        if (roles.Any())
                        {
                            // SuperUsers can do anything
                            var userIsSuperUser = await _rolesService.UserIsSuperUserAsync(appUser.Id);
                            if (!userIsSuperUser)
                            {
                                var permissions = await _rolesService.GetPermissionsByUserIdAsync(appUser.Id);

                                

                                // check that role contains the method and endpoint that the user is requesting
                                var permissionList = permissions as IList<Permission> ?? permissions.ToList();

                                var allowed = permissionList.Where(p => p.Method == method).Any(permission => PathContains(path, permission.Endpoint.ToLower()));

                                if (!allowed)
                                {
                                    context.Response.OnSendingHeaders(state =>
                                    {
                                        var resp = (OwinResponse) state;
                                        resp.StatusCode = 401;
                                        resp.ReasonPhrase = "RBAC error: user unauthorized to access " + path;
                                    }, context.Response);
                                    return;
                                }
                            }
                        }
                    
                }
                await Next.Invoke(context);
            }

        }

        private static bool PathContains(string fullPath, string fullEndpoint)
        {
            var pathWithoutQuestionMark = fullPath.Split('?').ToArray()[0];
            var result = false;
            var pathSplitted = pathWithoutQuestionMark.Split('/').ToArray();
            var endpointSplitted = fullEndpoint.Split('/').ToArray();

            // If the number of parts of the path and the endpoint are different, this is not the endpoint to check
            if (pathSplitted.Count() != endpointSplitted.Count()) 
                return false;
            
            foreach (var endpoint in endpointSplitted)
            {
                if (!endpoint.Contains(':'))
                {
                    result = pathSplitted.Any(x => x == endpoint);

                    if (!result) break;
                }
            }

            return result;
        }
    }
}