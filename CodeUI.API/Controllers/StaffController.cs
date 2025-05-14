using CodeUI.Service.DTO.Request.StaffRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.StaffResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    [Authorize(Roles ="Moderator,SystemAdmin")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        ///<summary>
        /// Login for staff
        /// </summary>
        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> Login([FromBody] CreateStaffRequest request)
        {
            try
            {
                var result = await _staffService.Login(request.Username, request.Password);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Reset password if no "oldPassword" - Change password otherwise
        /// </summary>
        [HttpPut("/updatePassword")]
        public async Task<ActionResult<BaseResponseViewModel<bool>>> UpdatePassword([Required]string password, string oldPassword = null)
        {
            string ID = null;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            ID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (ID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _staffService.UpdatePassword(ID, password,oldPassword);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Send email to reset password
        /// </summary>
        [HttpGet("/forgotPassword")]
        public async Task<ActionResult> ForgotPassword()
        {
            string ID = null;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            ID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (ID == null)
            {
                return Unauthorized();
            }
            try
            {
                await _staffService.ForgotPassword(ID);
                return Ok("Mail sent!");
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
