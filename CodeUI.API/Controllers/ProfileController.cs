using CodeUI.Service.DTO.Request.ProfileRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ProfileResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        ///<summary>
        /// Update a profile by account id
        /// </summary>
        [HttpPut("updateById")]
        [Authorize]
        public async Task<ActionResult<BaseResponseViewModel<ProfileResponse>>> UpdateProfile(UpdateProfileRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountId == null)
                {
                    return Unauthorized();
                }
            try
            {
                var result = await _profileService.UpdateProfile(accountId, request);
                return Ok(result);
            } 
            catch (ErrorResponse ex) 
            {
                return BadRequest(ex.Error); 
            }
        }

        ///<summary>
        /// Get a profile by account id
        /// </summary>
        [HttpGet("getByAccountId")]
        public async Task<ActionResult<BaseResponseViewModel<ProfileResponse>>> GetProfileByToken([FromQuery]string accountId = null)
        {
            try
            {
                if (accountId == null)
                {
                    var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                    if (accountId == null)
                    {
                        return Unauthorized();
                    }
                }

                var result = await _profileService.GetProfileByAccountId( accountId );
                return Ok(result);
            } 
            catch(ErrorResponse ex) 
            { 
                return BadRequest(ex.Error); 
            }
        }
        
        ///<summary>
        /// Get a profile by username
        /// </summary>
        [HttpGet("getByUsername")]
        public async Task<ActionResult<BaseResponseViewModel<ProfileResponse>>> GetProfileByUsername([FromQuery] string username)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);

                var result = await _profileService.GetProfileByUsername(accountId, username);
                return Ok(result);
            } 
            catch(ErrorResponse ex) 
            { 
                return BadRequest(ex.Error); 
            }
        }
    }
}
