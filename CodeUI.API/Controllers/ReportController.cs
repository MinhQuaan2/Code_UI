using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ReportRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static CodeUI.Service.Helpers.Enum;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        //retry deploying
        public readonly IReportService reportService;
        public ReportController(IReportService _reportService)
        {
            reportService = _reportService;
        }
        [HttpGet("availableElementReasons")]
        [AllowAnonymous]
        public IActionResult GetAvailableElementReasons()
        {
            var reasons = Enum.GetNames(typeof(ReportElementReasonEnum)).Select(name => new { Name = name, Value = (int)Enum.Parse(typeof(ReportElementReasonEnum), name) });
            return Ok(reasons);
        }
        [HttpGet("availableAccountReasons")]
        [AllowAnonymous]
        public IActionResult GetAvailableAccountReasons()
        {
            var reasons = Enum.GetNames(typeof(ReportAccountReasonEnum)).Select(name => new { Name = name, Value = (int)Enum.Parse(typeof(ReportAccountReasonEnum), name) });
            return Ok(reasons);
        }
        [HttpPost("createFulfillmentReport")]
        public async Task<ActionResult<BaseResponseViewModel<FulfillmentReportResponse>>> CreateFulfillmentReport([FromQuery][Required] int fulfillmentId, [FromBody][Required] FulfillmentReportRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.createFulfillmentReport(Guid.Parse(accountID), fulfillmentId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("createElementReport")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResponse>>> CreateElementReport([FromQuery][Required] int elementId, [FromBody][Required] ReportRequest request, [Required][FromQuery] ReportElementReasonEnum reason)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.createElementReport(Guid.Parse(accountID), elementId, request, reason);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        //[HttpPost("createElementReport")]
        //public async Task<ActionResult<BaseResponseViewModel<ReportResponse>>> CreateElementReport([FromQuery][Required] int elementId, [FromBody][Required] ReportRequest request, [FromQuery] ReportElementReasonEnum reason)
        //{
        //    var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
        //    if (accountID == null)
        //    {
        //        return Unauthorized();
        //    }
        //    try
        //    {
        //        var result = await reportService.createElementReport(Guid.Parse(accountID), elementId, request, reason);
        //        return Ok(result);
        //    }
        //    catch(ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}
        [HttpPost("createAccountReport")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResponse>>> CreateAccountReport([FromQuery][Required] string username, [FromBody][Required] ReportRequest request, [Required][FromQuery]ReportAccountReasonEnum reason)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.createAccountReport(Guid.Parse(accountID), username, request, reason);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getElementReport")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ReportResponse>>> GetElementReports([FromQuery] int elementId, [FromQuery] PagingRequest paging, [FromQuery]GetReportRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.getAllElementReport(paging, request, elementId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getAccountReport")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ReportResponse>>> GetAccountReports([FromQuery] string? username, [FromQuery] PagingRequest paging, [FromQuery] GetReportRequest request)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.getAllAccountReport(paging, request, username);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getOwnFulfillmentReport")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FulfillmentReportResponse>>> GetOwnFulfillmentReports([FromQuery] PagingRequest paging)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await reportService.getOwnFulfillmentReport(paging, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
