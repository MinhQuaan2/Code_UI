using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.AccountRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
            //
        }

        ///<summary>
        /// Get account by access token
        /// </summary>
        /// <param name="accountID"> Get specific accountID other than current account </param>
        [HttpGet("getByAccessToken")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> GetAccountByToken(string accountID = null)
        {
            if(accountID == null) { 
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountID == null)
                {
                    return Unauthorized();
                }
            }
            try
            {
                var result = await _accountService.GetAccountByToken(accountID);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        /// <summary>
        /// Google Login
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("loginByMail")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> LoginByMail([FromBody] ExternalAuthRequest data)
        {
            try
            {
                var result = await _accountService.LoginByMail(data);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Register a new account
        ///</summary>
        ///<param name="request"></param>
        ///<returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> Register([FromBody] CreateAccountRequest request)
        {
            try
            {
                var result = await _accountService.CreateAccount(request);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get all accounts with filter
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<MultipleAccountResponse>>> GetAll([FromQuery]MultipleAccountResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _accountService.GetAccounts(filter, paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get top accounts with most elements
        /// </summary>
        [HttpGet("topAccounts")]
        public async Task<ActionResult<BaseResponsePagingViewModel<TopAccountResponse>>> GetTopAccounts([FromQuery] PagingRequest paging, [FromQuery] int amount = 6)
        {
            try
            {
                var result = await _accountService.GetTopAccountByElementCount(amount, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
