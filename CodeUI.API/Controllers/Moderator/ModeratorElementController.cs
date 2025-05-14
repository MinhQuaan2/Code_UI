using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Moderator
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class ModeratorElementController : ControllerBase
    {
        private readonly IModeratorElementService _moderatorElementService;

        public ModeratorElementController(IModeratorElementService moderatorElementService)
        {
            _moderatorElementService = moderatorElementService;
        }

        ///<summary>
        /// Get all PENDING elements with filter
        /// </summary>
        [HttpGet("getPendingElements")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ElementResponse>>> GetPendingElements([FromQuery] ElementResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                filter.Status = filter.Status?.ToUpper();
                var result = await _moderatorElementService.GetPendingElements(filter, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Update multiple elements' statuses at the same time
        /// </summary>
        [HttpPut("updateElementStatus")]
        public async Task<ActionResult<List<ElementResponse>>> UpdateElementStatus(UpdateStatusRequest request)
        {
            try
            {
                var result = await _moderatorElementService.UpdateElementStatuses(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Delete all inactive elements
        /// </summary>
        [HttpDelete("deleteInactiveElements")]
        public async Task<ActionResult> DeleteInactiveElements()
        {
            try
            {
                await _moderatorElementService.DeleteInactiveElements();
                return NoContent();
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
