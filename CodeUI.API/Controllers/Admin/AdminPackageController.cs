using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeUI.API.Controllers.Admin
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    //[Authorize(Roles = "SystemAdmin")]
    public class AdminPackageController : ControllerBase
    {
        private readonly IAdminPackageService _adminPackageService;

        public AdminPackageController(IAdminPackageService adminPackageService)
        {
            _adminPackageService = adminPackageService;
        }

        ///<summary>
        /// Get a certain month revenue stat, with comparison to prev month
        /// </summary>
        [HttpGet("getMonthlyRevenue")]
        public async Task<ActionResult<BaseResponseViewModel<PackageDashboardResponse>>> GetMonthlyRevenue([FromQuery]int month, [FromQuery]int year)
        {
            try
            {
                var result = await _adminPackageService.GetDashboardPackage(month,year);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
