using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.Service.ModeratorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CodeUI.Service.Exceptions;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.DTO.Request.ReportRequest;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Response.AccountResponses;

namespace CodeUI.API.Controllers.Moderator
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class ModeratorReportController : ControllerBase
    {
        private readonly IModeratorReportService _moderatorReportService;
        public ModeratorReportController(IModeratorReportService moderatorReportService)
        {
            _moderatorReportService = moderatorReportService;
        }
        //[HttpGet("getFulfillmentReport")]
        //public async Task<ActionResult<BaseResponsePagingViewModel<ModFulfillmentReportResponse>>> GetAllFulfillmentReport([FromQuery] PagingRequest paging)
        //{
        //    try
        //    {
        //        var result = await _moderatorReportService.getPendingElementReport(paging);
        //        return Ok(result);
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}
        //[Authorize(Roles = "SystemAdmin, Moderator")]
        [HttpPost("rejectFulfillmentReport")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResultResponse>>> RejectFulfillmentReport([FromQuery][Required] int id, [FromBody] ResponseRequest request)
        {
            try
            {
                var result = await _moderatorReportService.rejectFulfillmentReport(id, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("approveFulfillmentReport")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResultResponse>>> ApproveFulfillmentReport([FromQuery][Required] int id)
        {
            try
            {
                var result = await _moderatorReportService.approveFulfillmentReport(id);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getFulfillmentReport")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ModFulfillmentReportResponse>>> GetAllFulfillmentReport([FromQuery]PagingRequest paging, [FromQuery]ReportStatusFilterRequest filter)
        {
            try
            {
                var result = await _moderatorReportService.getAllFulfillmentReport(paging, filter);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getPendingElementReports")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ModElementReportResponse>>> GetPendingElementReports([FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _moderatorReportService.getPendingElementReport(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("approveElementReport")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResultResponse>>> ApproveElementReport([FromQuery][Required] int Id)
        {
            try
            {
                var result = await _moderatorReportService.approveElementReport(Id);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("rejectReport")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponseViewModel<ReportResultResponse>>> RejectReport([FromQuery][Required] int Id, [FromBody] ResponseRequest request)
        {
            try
            {
                var result = await _moderatorReportService.rejectReport(Id, request);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpGet("getByElementId")]
        [Authorize(Roles = "Moderator, SystemAdmin")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ReportResponse>>> GetByElementId([FromQuery][Required] int elementId, [FromQuery]PagingRequest paging, [FromQuery] ReportStatusFilterRequest filter)
        {
            try
            {
                var result = await _moderatorReportService.getByElementId(elementId, paging, filter);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpGet("getPendingAccountReports")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ModElementReportResponse>>> GetPendingAccountReports([FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _moderatorReportService.getPendingAccountReport(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Block a user's account
        /// </summary>
        /// <param name="ID">Account ID</param>
        /// <returns></returns>
        [HttpDelete("blockAccount")]
        public async Task<ActionResult<BaseResponseViewModel<BlockedAccountResponse>>> BlockAccount([FromRoute] string ID)
        {
            try 
            {
                var result = await _moderatorReportService.blockAccount(ID);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Unblock a user's account
        /// </summary>
        /// <param name="ID"> Account ID </param>
        [HttpPut("unblockAccount")]
        public async Task<ActionResult<BaseResponseViewModel<BlockedAccountResponse>>> UnblockAccount([FromRoute] string ID)
        {
            try
            {
                var result = await _moderatorReportService.unblockAccount(ID);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
