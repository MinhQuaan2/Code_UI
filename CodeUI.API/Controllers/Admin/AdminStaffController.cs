using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.StaffRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.StaffResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Admin
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    //[Authorize(Roles ="SystemAdmin")]
    public class AdminStaffController : ControllerBase
    {
        private readonly IAdminStaffService _adminStaffService;

        public AdminStaffController(IAdminStaffService adminStaffService)
        {
            _adminStaffService = adminStaffService;
        }

        ///<summary>
        /// Create moderators' accounts
        /// </summary>
        [HttpPost("/createAccount")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> CreateModAccount([FromBody]CreateStaffRequest request)
        {
            try
            {
                var result = await _adminStaffService.CreateModAccount(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Update other info for staff
        /// </summary>
        [HttpPut("/updateInfo")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> UpdateInfo(string ID, UpdateStaffRequest request)
        {
            try
            {
                var result = await _adminStaffService.UpdateStaffInfo(ID,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Get all mod accounts
        /// </summary>
        [HttpGet("/getAllMod")]
        public async Task<ActionResult<BaseResponsePagingViewModel<QueryStaffRequest>>> GetAllMod([FromQuery]QueryStaffRequest request, [FromQuery]PagingRequest paging)
        {
            try
            {
                var result = await _adminStaffService.GetAllStaffAccount(request, paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        
        ///<summary>
        /// Get all mod accounts
        /// </summary>
        [HttpGet("/getAllMod")]
        public async Task<ActionResult<BaseResponseViewModel<QueryStaffRequest>>> GetSingleMod(string ID)
        {
            try
            {
                var result = await _adminStaffService.GetSingleStaffAccount(ID);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Disable staff account
        /// </summary>
        [HttpDelete("disableMod")]
        public async Task<ActionResult> DisableMod([FromQuery]string ID)
        {
            try
            {
                await _adminStaffService.DisableStaff(ID);
                return Ok();
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
