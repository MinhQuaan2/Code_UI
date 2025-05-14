using CodeUI.Service.DTO.Request.RequestRequest;
using CodeUI.Service.DTO.Response.RequestResponse;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CodeUI.Service.Exceptions;
using CodeUI.Service.DTO.Request;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Identity.Client;
using CodeUI.Service.Helpers;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService requestService;
        public RequestController(IRequestService requestService)
        {
            this.requestService = requestService;
        }
        [HttpDelete("deleteRequestInDatabase")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> DeleteRequestInDB([FromQuery] int requestId)
        {
            try
            {
                var result = await requestService.deleteRequestInDatabase(requestId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("createRequest")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> CreateRequest([FromBody] RequestRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.createRequest(Guid.Parse(accountID), request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("updateRequest")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> UpdateRequest([FromQuery] int requestId, [FromBody] UpdateRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.updateRequest(requestId, request, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("cancelRequest")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> CancelRequest([FromQuery] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.cancelRequest(requestId, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getRequestList")]
        public async Task<ActionResult<BaseResponsePagingViewModel<RequestResponse>>> GetRequestList([FromQuery] PagingRequest paging, [FromQuery] SortAndFilterRequest request, [FromQuery] string accountID = null, [FromQuery] string requesterId = null)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            try
            {
                var result = await requestService.getRequestList(paging, request, accountID, requesterId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getRequestById")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> GetRequestById([FromQuery] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            try
            {
                var result = await requestService.getRequestById(requestId, accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("acceptRequest")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> AcceptRequest([FromQuery] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.acceptRequest(requestId, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("giveUpRequest")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> GiveUpRequest([FromQuery] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.giveUpRequest(requestId, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFulfillmentByRequestId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FulfillmentResponse>>> GetFulfillmentByRequestId([FromQuery] int requestId, [FromQuery] PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.getFulfillmentByRequestId(requestId, Guid.Parse(accountID), paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getProcessFulfillment")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FulfillmentResponse>>> GetProcessFulfillment([FromQuery] PagingRequest paging, [FromQuery][Required] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.getProcessFulfillment(Guid.Parse(accountID), paging, requestId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getOwnFulfillment")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FulfillmentResponse>>> GetOwnFulfillment([FromQuery] PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.getOwnFulfillment(Guid.Parse(accountID), paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getOwnFulfillmentByRequestId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FulfillmentResponse>>> GetOwnFulfillmentByRequestId([FromQuery] PagingRequest paging, [FromQuery] int requestId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.getOwnFulfillmentByRequestId(Guid.Parse(accountID), paging, requestId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("acceptFulfillment")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> AcceptFulfillment([FromQuery] int fulfillmentId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.AcceptFulfillment(fulfillmentId, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        /// <summary>
        /// Reject fulfillment.
        /// </summary>
        /// <param name="fulfillmentId">ID of the fulfillment.</param>
        /// <param name="reason">Reason for rejection.</param>
        /// <returns>Result of the rejection operation.</returns>
        [HttpPut("rejectFulfillment")]
        public async Task<ActionResult<BaseResponseViewModel<RequestResponse>>> RejectFulfillment([FromQuery] int fulfillmentId, [FromBody][Required] string reason)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.RejectFulfillment(fulfillmentId, Guid.Parse(accountID), reason);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("submitFulfillment")]
        public async Task<ActionResult<BaseResponseViewModel<FulfillmentResponse>>> submitFulfillment([FromQuery] int fulfillmentId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.submitFulfillment(Guid.Parse(accountID), fulfillmentId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFulfillmentDetailById")]
        public async Task<ActionResult<BaseResponseViewModel<FulfillmentDetailResponse>>> GetFulfillmentDetailById([FromQuery] int fulfillmentId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await requestService.getFulfillmentDetailById(fulfillmentId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
