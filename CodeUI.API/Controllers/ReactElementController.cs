using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ReactRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using CodeUI.Service.DTO.Response.ReactElementResponses;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    [Authorize]
    public class ReactElementController : ControllerBase
    {
        private readonly IReactElementService reactElementService;
        public ReactElementController(IReactElementService reactElementService)
        {
            this.reactElementService = reactElementService;
        }
        [HttpPost("likeElement")]
        public async Task<ActionResult<BaseResponseViewModel<LikeResponse>>> LikeElement(int ElementId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.likeElement(ElementId, Guid.Parse(IdToken));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("createComment")]
        public async Task<ActionResult<BaseResponseViewModel<CommentResponse>>> CreateComment(int ElementId, CommentRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.createComment(ElementId, Guid.Parse(IdToken), request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("saveFavorite")]
        public async Task<ActionResult<BaseResponseViewModel<FavoriteResponse>>> SaveFavorite(int ElementId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.saveFavorite(ElementId, Guid.Parse(IdToken));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("replyComment")]
        public async Task<ActionResult<BaseResponseViewModel<CommentResponse>>> ReplyComment(int CommentId, CommentRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.replyComment(CommentId, Guid.Parse(IdToken), request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("getCommentsByElementId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CommentResponse>>> GetCommentsByElementId([Required]int ElementId, [FromQuery]PagingRequest paging)
        {
            try
            {
                var result = await reactElementService.getCommentsByElementId(ElementId, paging);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("editComment")]
        public async Task<ActionResult<BaseResponseViewModel<CommentResponse>>> EditComment([FromQuery]int CommentId, [FromBody]CommentRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.editComment(CommentId, Guid.Parse(IdToken), request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("deleteComment")]
        public async Task<ActionResult<BaseResponseViewModel<CommentResponse>>> DeleteComment([FromQuery] int CommentId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var IdToken = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (IdToken == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reactElementService.deleteComment(CommentId, Guid.Parse(IdToken));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
