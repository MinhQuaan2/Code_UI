using AutoMapper;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Request.PackageRequest;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeUI.Service.DTO.Request.DonationRequest;
using CodeUI.Service.DTO.Response.DonationResponse;
using CodeUI.Data.Entity;
using CodeUI.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using static CodeUI.Service.Helpers.ErrorEnum;
using Grpc.Core;
using CodeUI.Service.Utilities;
using AutoMapper.QueryableExtensions;
using System.Net.NetworkInformation;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.Attributes;
using Microsoft.Identity.Client;

namespace CodeUI.Service.Service
{
    public interface IDonationService
    {
        Task<BaseResponseViewModel<DonationPackageResponse>> createDonationPackage(DonationPackageRequest request, Guid creatorId);
        Task<BaseResponseViewModel<DonationPackageResponse>> updateDonationPackage(DonationPackageUpdateRequest request, int packageId);
        Task<BaseResponseViewModel<DonationPackageResponse>> deleteDonationPackage(int packageId);
        Task<BaseResponseViewModel<DonationBenefitResponse>> createDonationBenefit(DonationBenefitRequest request);
        Task<BaseResponseViewModel<DonationPackageDetailResponse>> addBenefitToPackage(int packageId, List<int> benefitIdList);
        Task<BaseResponsePagingViewModel<DonationPackageResponse>> getDonationPackageByAccountId(Guid accountId, PagingRequest paging);
        Task<BaseResponsePagingViewModel<DonationBenefitResponse>> getDonationBenefit(PagingRequest paging);
        Task<BaseResponseViewModel<DonationPackageDetailResponse>> getDonationPackageDetail(int packageId);
        Task<BaseResponseViewModel<DonationPackageDetailResponse>> addConfig(ConfigRequest request, int benefitId, int packageId);
    }
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public DonationService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        public async Task<BaseResponseViewModel<DonationPackageResponse>> createDonationPackage(DonationPackageRequest request, Guid creatorId)
        {
            try
            {
                var creator = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == creatorId)
                    .FirstOrDefaultAsync();
                if(creator == null)
                {
                    throw new ErrorResponse(404, 
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var package = _mapper.Map<DonationPackage>(request);
                package.IsActive = true;
                package.Owner = creator;
                package.OwnerId = creatorId;
                package.DonationPackageDetail.CreateDate = DateTime.Now;
                package.DonationPackageDetail.UpdateDate = DateTime.Now;
                await _unitOfWork.Repository<DonationPackage>().InsertAsync(package);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<DonationPackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<DonationPackageResponse>(package)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationPackageResponse>> updateDonationPackage(DonationPackageUpdateRequest request, int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<DonationPackage>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND.GetDisplayName());
                }
                package.Description = request.Description;
                package.DonationPackageDetail.ImageUrl = request.ImageUrl;
                package.DonationPackageDetail.UpdateDate = DateTime.Now;
                await _unitOfWork.Repository<DonationPackage>().UpdateDetached(package);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<DonationPackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<DonationPackageResponse>(package)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationPackageResponse>> deleteDonationPackage(int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<DonationPackage>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND.GetDisplayName());
                }
                package.IsActive = false;
                await _unitOfWork.Repository<DonationPackage>().UpdateDetached(package);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<DonationPackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<DonationPackageResponse>(package)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationBenefitResponse>> createDonationBenefit(DonationBenefitRequest request)
        {
            try
            {
                var benefit = _mapper.Map<DonationBenefit>(request);
                await _unitOfWork.Repository<DonationBenefit>().InsertAsync(benefit);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<DonationBenefitResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<DonationBenefitResponse>(benefit)
                };
            }
            catch (ErrorResponse)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationPackageDetailResponse>> addBenefitToPackage(int packageId, List<int> benefitIdList)
        {
            try
            {
                var package = await _unitOfWork.Repository<DonationPackage>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND.GetDisplayName());
                }
                var packageDetail = await _unitOfWork.Repository<DonationPackageDetail>().GetAll()
                    .Where(p => p.Id == package.Id)
                    .FirstOrDefaultAsync();
                DonationPackageDetailResponse response = await getDonationPackageDetailById(packageId);
                if (packageDetail == null)
                {
                    throw new ErrorResponse(404, (int)DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND.GetDisplayName());
                }
                if (benefitIdList.Count > 0)
                {
                    benefitIdList = benefitIdList.Distinct().OrderBy(id => id).ToList();
                    var allBenefits = await _unitOfWork.Repository<DonationBenefit>().GetAll().ToListAsync();
                    var allBenefitIds = allBenefits.Select(f => f.Id).ToList();
                    var detailBenefit = await _unitOfWork.Repository<DonationDetailBenefit>().GetAll().ToListAsync();
                    var existingDetailBenefitIds = detailBenefit
                        .Where(pf => pf.DonationDetailId == packageId)
                        .Select(pf => pf.DonationBenefitId)
                        .ToList();
                    var missingBenefitIds = benefitIdList.Except(allBenefitIds).ToList();

                    var benefitIdsChange = benefitIdList.SequenceEqual(existingDetailBenefitIds.Cast<int>());

                    if (missingBenefitIds.Any())
                    {
                        throw new ErrorResponse(
                            400, (int)DonationPackageErrorEnum.DONATION_BENEFIT_NOT_FOUND,
                            $"Some feature IDs do not exist: {string.Join(", ", missingBenefitIds)}");
                    }


                    if (benefitIdsChange)
                    {
                        return new BaseResponseViewModel<DonationPackageDetailResponse>()
                        {
                            Status = new StatusViewModel()
                            {
                                Message = "Nothing changes",
                                Success = true,
                                ErrorCode = 0
                            },
                            Data = response
                        };
                    }
                    var detailBenefitToDelete = detailBenefit
                        .Where(pf => pf.DonationDetailId == packageId)
                        .ToList();
                    foreach (var dB in detailBenefitToDelete)
                    {
                        _unitOfWork.Repository<DonationDetailBenefit>().Delete(dB);
                    }

                    var benefitIdListNullable = benefitIdList.Select(id => (int?)id).ToList();
                    //var existingPackageFeatureIdsNullable = existingPackageFeatureIds.ToList();

                    var benefitsToAdd = allBenefits
                        //.Where(f => featureIdListNullable.Except(existingPackageFeatureIdsNullable).Contains(f.Id))
                        .Where(f => benefitIdListNullable.Contains(f.Id))
                        .ToList();

                    var detailBenefits = benefitsToAdd.Select(benefit => new DonationDetailBenefit
                    {
                        DonationBenefit = benefit,
                        DonationDetail = packageDetail,
                        DonationBenefitId = benefit.Id,
                        DonationDetailId = benefit.Id,
                    }).ToList();
                    foreach (var dB in detailBenefits)
                    {
                        await _unitOfWork.Repository<DonationDetailBenefit>().InsertAsync(dB);
                    }
                }
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<DonationPackageDetailResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<DonationPackageResponse>> getDonationPackageByAccountId(Guid accountId, PagingRequest paging)
        {
            try
            {
                var donationPackage = _unitOfWork.Repository<DonationPackage>().GetAll()
                    .Where(p => p.IsActive == true && p.OwnerId == accountId)
                    .ProjectTo<DonationPackageResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<DonationPackageResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = donationPackage.Item1
                    },
                    Data = donationPackage.Item2.ToList()
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<DonationBenefitResponse>> getDonationBenefit(PagingRequest paging)
        {
            try
            {
                var donationBenefit = _unitOfWork.Repository<DonationBenefit>().GetAll()
                    .ProjectTo<DonationBenefitResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<DonationBenefitResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = donationBenefit.Item1
                    },
                    Data = donationBenefit.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationPackageDetailResponse>> getDonationPackageDetail(int packageId)
        {
            try
            {
                var response = await getDonationPackageDetailById(packageId);
                return new BaseResponseViewModel<DonationPackageDetailResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<DonationPackageDetailResponse>> addConfig(ConfigRequest request, int benefitId, int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<DonationPackage>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_PACKAGE_NOT_FOUND.GetDisplayName());
                }
                var detailBenefit = await _unitOfWork.Repository<DonationDetailBenefit>()
                    .GetAll()
                    .Where(pf => pf.DonationBenefitId == benefitId && pf.DonationDetailId == package.Id)
                    .FirstOrDefaultAsync();
                if(detailBenefit == null)
                {
                    throw new ErrorResponse(404, (int)DonationPackageErrorEnum.DONATION_BENEFIT_NOT_FOUND,
                        DonationPackageErrorEnum.DONATION_BENEFIT_NOT_FOUND.GetDisplayName());
                }
                detailBenefit.Number = request.Number;
                detailBenefit.Config = request.ConfigName;
                detailBenefit.Unit = request.Unit;
                await _unitOfWork.Repository<DonationDetailBenefit>().UpdateDetached(detailBenefit);
                await _unitOfWork.CommitAsync();

                var response = await getDonationPackageDetailById(packageId);
                return new BaseResponseViewModel<DonationPackageDetailResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        private async Task<DonationPackageDetailResponse> getDonationPackageDetailById(int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<DonationPackageDetail>().GetAll()
                    .Include(p => p.DonationDetailBenefits)
                    .ThenInclude(pf => pf.DonationBenefit)
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();

                // Check if the package exists
                if (package != null)
                {
                    // Use AutoMapper to map the Package entity to PackageResponse
                    var packageDetailResponse = _mapper.Map<DonationPackageDetailResponse>(package);

                    return packageDetailResponse;
                }

                // Handle the case where the package with the given ID doesn't exist
                return null; // or throw an ErrorResponse or return an appropriate response
            }
            catch (ErrorResponse ex)
            {
                // Handle ErrorResponses
                // Log the error or return an appropriate response
                throw;
            }
        }

    }
}
