using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.PackageRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService packageService;
        public PackageController(IPackageService packageService)
        {
            this.packageService = packageService;
        }
        [HttpDelete("deletePackageInDatabase")]
        public async Task<ActionResult<BaseResponseViewModel<PackageResponse>>> DeletePackageInDatabase(int packageId)
        {
            try
            {
                var result = await packageService.deletePackageInDatabase(packageId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        [HttpPut("changePackageStatus")]
        public async Task<ActionResult<BaseResponseViewModel<PackageResponse>>> ChangePackageStatus(int packageId, [Required]bool status)
        {
            try
            {
                var result = await packageService.changePackageStatus(packageId, status);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getPackageToShow")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PackageResponse>>> getPackageToShow([FromQuery]PagingRequest paging)
        {
            string accountID = null;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            try
            {
                var result = await packageService.getPackageToShow(paging, accountID);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("buyPackage")]
        public async Task<ActionResult<BaseResponseViewModel<BuyPackageResponse>>> BuyPackage(int packageId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
            if (accountID == null)
            {
                return Unauthorized();
            }
            try
            {
                var result = await packageService.buyPackage(packageId, Guid.Parse(accountID));
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("createPackage")]
        public async Task<ActionResult<BaseResponseViewModel<PackageResponse>>> CreatePackage(PackageRequest request)
        {
            try
            {
                var result = await packageService.createPackage(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }   
        }
        [HttpPost("createFeature")]
        public async Task<ActionResult<BaseResponseViewModel<FeatureResponse>>> CreateFeature([Required]FeatureRequest request)
        {
            try
            {
                var result = await packageService.createFeature(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("updateFeature")]
        public async Task<ActionResult<BaseResponseViewModel<FeatureResponse>>> UpdateFeature([Required] FeatureRequest request, [Required] int packageId)
        {
            try
            {
                var result = await packageService.updateFeature(request, packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("deleteFeatureByChangeStatus")]
        public async Task<ActionResult<BaseResponseViewModel<FeatureResponse>>> DeleteFeature([Required] int packageId, [Required]bool status)
        {
            try
            {
                var result = await packageService.changePackageStatus(packageId, status);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpDelete("deleteFeatureInDatabase")]
        public async Task<ActionResult<BaseResponseViewModel<FeatureResponse>>> DeleteFeatureInDatabase([Required] int packageId)
        {
            try
            {
                var result = await packageService.deleteFeatureInDatabase(packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("addFeatureToPackage")]
        public async Task<ActionResult<BaseResponseViewModel<FeatureResponse>>> AddFeatureToPackage([FromQuery] int packageId, [FromBody] List<int> featureIdList)
        {
            try
            {
                var result = await packageService.addFeaturesToPackage(packageId, featureIdList);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getAllPackages")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PackageResponse>>> GetAllPackages([FromQuery] PagingRequest paging)
        {
            
            try
            {
                var result = await packageService.getAllPackages(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getAllFeatures")]
        public async Task<ActionResult<BaseResponsePagingViewModel<FeatureResponse>>> GetAllFeature([FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await packageService.getAllFeatures(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("addConfig")]
        public async Task<ActionResult<BaseResponseViewModel<PackageResponse>>> AddConfig([FromBody]ConfigRequest request, [FromQuery][Required]int packageId, [FromQuery][Required]int featureId)
        {
            try
            {
                var result = await packageService.createConfig(request, featureId, packageId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
