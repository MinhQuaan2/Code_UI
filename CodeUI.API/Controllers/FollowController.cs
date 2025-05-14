using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.FollowResponse;
using CodeUI.Service.DTO.Response.FollowResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowCreatorService _followService;
        public FollowController(IFollowCreatorService followCreatorService)
        {
            _followService = followCreatorService;
        }
        [HttpPost("followCreator")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponseViewModel<FollowResponse>>> FollowCreator([FromQuery][Required] String username)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.followCreator(username, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFollowing")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FollowAccountResponse>>> GetFollowing([FromQuery] PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.getFollowing(paging, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFollower")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FollowAccountResponse>>> GetFollower([FromQuery] PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.getFollower(paging, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFollowingByUsername")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FollowAccountResponse>>> GetFollowingByUsername([FromQuery] PagingRequest paging, [FromQuery][Required] string username)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.getFollowingByUsername(paging, username);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFollowerByUsername")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FollowAccountResponse>>> GetFollowerByUsername([FromQuery] PagingRequest paging, [FromQuery][Required] string username)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.getFollowerByUsername(paging, username);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getAllFollowByUsername")]
        [Authorize(Roles = "FreeCreator, PaidCreator")]
        public async Task<ActionResult<BaseResponseViewModel<FollowListResponse>>> GetAllFollowByUsername([FromQuery][Required] string username)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await _followService.getAllFollowByUsername(username);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
