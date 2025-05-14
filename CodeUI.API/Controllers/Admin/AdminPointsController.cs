using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PointsResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service.AdminServices;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Admin
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class AdminPointsController : ControllerBase
    {
        private readonly IAdminPointsService _adminPointsService;

        public AdminPointsController(IAdminPointsService adminPointsService)
        {
            _adminPointsService = adminPointsService;
        }

        /// <summary>
        /// Returns a list of creators' points transactions, diff to prev month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("dashboardCreatorPoints")]
        public async Task<ActionResult<BaseResponseViewModel<DashboardPointsResponse>>> GetCreatorPointsDashboard([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _adminPointsService.GetCreatorPointsDashboard(month, year);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Returns a list of admin points gains, diff to prev month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("dashboardAdminPoints")]
        public async Task<ActionResult<BaseResponseViewModel<DashboardAdminPointsResponse>>> GetAdminPointsDashboard([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _adminPointsService.GetAdminPointsDashboard(month, year);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
