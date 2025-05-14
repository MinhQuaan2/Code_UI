using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Admin
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    //[Authorize(Roles = "SystemAdmin")]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAdminAccountService _accountService;

        public AdminAccountController(IAdminAccountService accountService)
        {
            _accountService = accountService;
        }

        ///<summary>
        /// Get all accounts
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdminAccountResponse>>> GetAllAccounts([FromQuery] AdminAccountResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _accountService.GetAllAccounts(filter,paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get all inactive accounts
        /// </summary>
        [HttpGet("getInactive")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountResponse>>> GetInactiveAccounts([FromQuery] AccountResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _accountService.GetInactiveAccounts(filter,paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get new accounts statistic from chosen month
        /// </summary>
        [HttpGet("getNewAccounts")]
        public async Task<ActionResult<DashboardNewAccountResponse>> GetNewAccounts([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _accountService.GetNewAccounts(month, year);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get Dashboard Statistic for new account from a certian month
        /// </summary>
        [HttpGet("getDashboardAccount")]
        public async Task<ActionResult<BaseResponseViewModel<DashboardAccountResponse>>> GetDashboardAccounts([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _accountService.GetDashboardAccount(month, year);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
