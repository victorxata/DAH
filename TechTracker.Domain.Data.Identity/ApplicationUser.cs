using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TechTracker.Domain.Data.Identity.Aspnet;

namespace TechTracker.Domain.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) 
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ExternalBearer);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType

            // BUG WHAT. If it blows up here, check if the SecurityStamp in the user's mongo object is null
            // it should be a guid
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);


            // Add custom user claims here
            return userIdentity;
        }


        [MaxLength(255)]
        [Required]
        public string RealName { get; set; }

        public bool Active { get; set; }


        public string ImageUrl { get; set; }
        
        public void ApplyChanges(RegisterModel model)
        {
            var myType = model.GetType();
            var props = new List<PropertyInfo>(myType.GetProperties());
            foreach (var prop in props)
            {
                if ((prop.Name.ToLower() == "id") || (prop.Name.ToLower() == "password")) 
                    continue;

                var pi = typeof(ApplicationUser).GetProperty(prop.Name);
                var piD = typeof(RegisterModel).GetProperty(prop.Name);

                if ((pi == null) || (piD == null))
                    continue;

                var val = piD.GetValue(model);
                pi.SetValue(this, val);
            }
        }
    }
}