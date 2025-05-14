using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CodeUI.Service.DTO.Response.DonationResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.DTO.Request.DonationRequest;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using CodeUI.Service.DTO.Request.PackageRequest;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class DonationController : ControllerBase
    {
        public readonly IDonationService donationService;
        public DonationController(IDonationService donationService)
        {
            this.donationService = donationService;
        }
        [HttpGet("getDonationPackageByAccountId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<DonationPackageResponse>>> GetAllDonationPackages([FromQuery] PagingRequest paging, [FromQuery][Required] Guid accountId)
        {
            try
            {
                var result = await donationService.getDonationPackageByAccountId(accountId, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("createDonationPackage")]
        public async Task<ActionResult<BaseResponseViewModel<DonationPackageResponse>>> CreateDonationPackage(DonationPackageRequest request, [Required]Guid accountId)
        {
            try
            {
                var result = await donationService.createDonationPackage(request, accountId);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("updateDonationPackage")]
        public async Task<ActionResult<BaseResponseViewModel<DonationPackageResponse>>> UpdateDonationPackage(DonationPackageUpdateRequest request, int packageId)
        {
            try
            {
                var result = await donationService.updateDonationPackage(request, packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpDelete("deleteDonationPackage")]
        public async Task<ActionResult<BaseResponseViewModel<DonationPackageResponse>>> DeleteDonationPackage(int packageId)
        {
            try
            {
                var result = await donationService.deleteDonationPackage(packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getDonationBenefits")]
        public async Task<ActionResult<BaseResponsePagingViewModel<DonationBenefitResponse>>> GetAllDonationBenefits([FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await donationService.getDonationBenefit(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("createDonationBenefit")]
        public async Task<ActionResult<BaseResponseViewModel<DonationBenefitResponse>>> CreateDonationBenefit(DonationBenefitRequest request)
        {
            try
            {
                var result = await donationService.createDonationBenefit(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        //[HttpPut("updateDonationBenefit")]
        //[HttpDelete("deleteDonationBenefit")]
        [HttpPut("addBenefitToDonationPackage")]
        public async Task<ActionResult<BaseResponsePagingViewModel<DonationPackageDetailResponse>>> AddBenefitToDonationPackage([FromQuery] int packageId, [FromBody] List<int> benefitIdList)
        {
            try
            {
                var result = await donationService.addBenefitToPackage(packageId, benefitIdList);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getDonationPackageDetail")]
        public async Task<ActionResult<BaseResponseViewModel<DonationPackageDetailResponse>>> GetDonationPackageDetail([FromQuery][Required] int packageId)
        {
            try
            {
                var result = await donationService.getDonationPackageDetail(packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPut("addConfig")]
        public async Task<ActionResult<BaseResponseViewModel<DonationPackageDetailResponse>>> AddConfig([Required]ConfigRequest request, [Required] int benefitId, [Required] int packageId)
        {
            try
            {
                var result = await donationService.addConfig(request, benefitId, packageId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
    
}
