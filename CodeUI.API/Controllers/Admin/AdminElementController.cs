using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Admin
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    //[Authorize(Roles = "SystemAdmin")]
    public class AdminElementController : ControllerBase
    {
        private readonly IAdminElementService _elementService;

        public AdminElementController(IAdminElementService elementService)
        {
            _elementService = elementService;
        }

        ///<summary>
        /// Get all new elements created THIS WEEK
        ///</summary>/>
        [HttpGet("getNewElements")]
        public async Task<ActionResult<DashboardElementResponse>> GetDashboardElements([FromQuery] ElementResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _elementService.GetDashboardElements(filter, paging);
                return Ok(result);
            }   
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
