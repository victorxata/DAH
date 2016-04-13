using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using TechTracker.Api.Common.Attributes;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Validation]
    [RoutePrefix("api")]
    public class AccountsController : BaseController
    {
        private readonly IAccountsService _accountsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="accountsService"></param>
        public AccountsController(ILog logger, IAccountsService accountsService) : base(logger)
        {
            _accountsService = accountsService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the accounts with all the properties that can be securized
        /// </summary>
        /// <returns>The complete list of accounts</returns>
        [HttpGet]
        [Route("accounts")]
        public async Task<IHttpActionResult> GetAccounts()
        {
            var result = await _accountsService.GetAccountsAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a account
        /// </summary>
        /// <param name="accountId">The Id of the account</param>
        /// <returns>The account that matches the given Id</returns>
        [HttpGet]
        [Route("accounts/{accountId}")]
        public async Task<IHttpActionResult> GetAccountById(string accountId)
        {
            var result = await _accountsService.GetAccountByIdAsync(accountId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new account. 
        /// </summary>
        /// <param name="account">The account object to add</param>
        /// <returns>201 Created response and a header with the location of the new account created</returns>
        [HttpPost]
        [Route("accounts")]
        public async Task<IHttpActionResult> PostAccount([FromBody] Account account)
        {
            var permissionTypeCreated = await _accountsService.AddAccountAsync(account);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing account.
        /// </summary>
        /// <param name="accountId">The Id of the account to retrieve</param>
        /// <param name="account">The object of the new account body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("accounts/{accountId}")]
        public async Task<IHttpActionResult> UpdateAccount(string accountId, [FromBody] Account account)
        {
            if (account.Id != accountId)
                return BadRequest("Field Permission Id does not match with request id");
            var result = await _accountsService.UpdateAccountAsync(account);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. 
        /// </summary>
        /// <param name="accountId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("accounts/{accountId}")]
        public async Task<IHttpActionResult> DeleteAccount(string accountId)
        {
            await _accountsService.DeleteAccountAsync(accountId);
            return Ok();
        }

    }
}
