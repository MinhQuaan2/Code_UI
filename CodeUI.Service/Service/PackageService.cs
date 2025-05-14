using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.PackageRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IPackageService
    {
        Task<BaseResponseViewModel<PackageResponse>> updatePackage(PackageRequest packageRequest, int packageId);
        Task<BaseResponseViewModel<PackageResponse>> changePackageStatus(int packageId, bool status);
        Task<BaseResponseViewModel<PackageResponse>> deletePackageInDatabase(int packageId);
        Task<BaseResponsePagingViewModel<PackageResponse>> getPackageToShow(PagingRequest paging, string accountId);
        Task<BaseResponseViewModel<BuyPackageResponse>> buyPackage(int packageId, Guid accountId);
        Task<BaseResponseViewModel<PackageResponse>> createPackage(PackageRequest request);
        Task<BaseResponseViewModel<FeatureResponse>> createFeature(FeatureRequest request);
        Task<BaseResponseViewModel<FeatureResponse>> updateFeature(FeatureRequest request, int featureId);
        Task<BaseResponseViewModel<FeatureResponse>> changeFeatureStatus(int featureId, bool status);
        Task<BaseResponseViewModel<FeatureResponse>> deleteFeatureInDatabase(int featureId);
        Task<BaseResponseViewModel<PackageResponse>> addFeaturesToPackage(int packageId, List<int> featureIdList);
        Task<BaseResponsePagingViewModel<PackageResponse>> getAllPackages(PagingRequest paging);
        Task<BaseResponsePagingViewModel<FeatureResponse>> getAllFeatures(PagingRequest paging);
        Task<BaseResponseViewModel<PackageResponse>> createConfig(ConfigRequest request, int featureId, int packageId);
        //Task<Package> GetPackageByIdAsync(int packageId);
    }
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public PackageService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        //public async Task<BaseResponseViewModel<>>
        public async Task<BaseResponseViewModel<PackageResponse>> updatePackage(PackageRequest request, int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<Package>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.PACKAGE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_NOT_FOUND.GetDisplayName());
                }
                package.Price = request.Price;
                package.Duration = request.Duration;
                package.Name = request.Name;
                await _unitOfWork.Repository<Package>().UpdateDetached(package);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<PackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PackageResponse>(package)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PackageResponse>> changePackageStatus(int packageId, bool status)
        {
            try
            {
                var package = await _unitOfWork.Repository<Package>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if(package == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.PACKAGE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_NOT_FOUND.GetDisplayName());
                }
                package.IsActive = status;
                await _unitOfWork.Repository<Package>().UpdateDetached(package);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<PackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PackageResponse>(package)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PackageResponse>> deletePackageInDatabase(int packageId)
        {
            try
            {
                var package = await _unitOfWork.Repository<Package>().GetAll()
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.PACKAGE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_NOT_FOUND.GetDisplayName());
                }
                package.IsActive = false;
                _unitOfWork.Repository<Package>().Delete(package);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<PackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PackageResponse>(package)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<PackageResponse>> getPackageToShow(PagingRequest paging, string accountId)
        {
            try
            {
                var packageList = _unitOfWork.Repository<Package>().GetAll()
                    .Include(p => p.PackageFeatures)
                    .ThenInclude(pf => pf.Feature)
                    .Where(p => p.IsActive == true)
                    .ProjectTo<PackageResponse>(_mapper.ConfigurationProvider)
                    .ToList();
                if (accountId != null)
                {
                    //.PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                    for (int i = 0; i < packageList.Count(); i++)
                    {
                        packageList[i] = await checkBoughtOrNot(packageList[i], Guid.Parse(accountId));
                    }
                }
                var response = packageList
                    .AsQueryable()
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<PackageResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = response.Item1
                    },
                    Data = response.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<BuyPackageResponse>> buyPackage(int packageId, Guid accountId)
        {
            try
            {
                var package = await _unitOfWork.Repository<Package>().GetAll()
                            .Where(p => p.Id == packageId)
                            .FirstOrDefaultAsync();
                if (package == null)
                {
                    throw new ErrorResponse(404, (int)PackageErrorEnum.PACKAGE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_NOT_FOUND.GetDisplayName());
                }
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == accountId)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                if (account.Profile.Wallet >= package.Price)
                {
                    account.Profile.Wallet -= package.Price;
                    PointsTransaction transaction = new PointsTransaction()
                    {
                        Account = account,
                        AccountId = accountId,
                        Amount = -package.Price,
                        Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                        Type = Helpers.Enum.PointTransactionTypeEnum.PACKAGE_BUY.GetDisplayName(),
                    };
                    AdminPoint point = new AdminPoint()
                    {
                        Account = account,
                        AccountId = accountId,
                        Amount = package.Price,
                        Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                        Type = Helpers.Enum.PointTransactionTypeEnum.PACKAGE_BUY.GetDisplayName(),
                    };
                    await _unitOfWork.Repository<PointsTransaction>().InsertAsync(transaction);
                    await _unitOfWork.Repository<AdminPoint>().InsertAsync(point);
                }
                else
                {
                    throw new ErrorResponse(400,
                    (int)PackageErrorEnum.NOT_ENOUGH_MONEY,
                    PackageErrorEnum.NOT_ENOUGH_MONEY.GetDisplayName());
                }
                var premiumList = _unitOfWork.Repository<PremiumNote>().GetAll().AsQueryable();
                var check = await checkPremiumNoteExisted(packageId, accountId);
                if (check == true)
                {
                    var premiumNote = await premiumList
                         .Where(pn => pn.PackageId == packageId && pn.AccountId == accountId && pn.IsActive == true)
                         .FirstOrDefaultAsync();
                    premiumNote.EndDate = TimeZoneInfo.ConvertTime(premiumNote.EndDate.AddMonths(package.Duration), _timeZoneInfo);
                    await _unitOfWork.Repository<PremiumNote>().UpdateDetached(premiumNote);
                    await _unitOfWork.Repository<Account>().UpdateDetached(account);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<BuyPackageResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<BuyPackageResponse>(premiumNote)
                    };
                }
                else
                {
                    var existedPremium = await premiumList
                        .Where(pn => pn.AccountId == accountId && pn.IsActive == true)
                        .ToListAsync();
                    foreach(var pn in existedPremium)
                    {
                        pn.IsActive = false;
                        await _unitOfWork.Repository<PremiumNote>().UpdateDetached(pn);
                    }
                    var premiumNote = new PremiumNote()
                    {
                        Account = account,
                        Package = package,
                        AccountId = accountId,
                        PackageId = packageId,
                        StartDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                        EndDate = TimeZoneInfo.ConvertTime(DateTime.Now.AddMonths(package.Duration), _timeZoneInfo),
                        IsActive = true
                    };
                    int roleId = 0;
                    if(package.Name == Helpers.Enum.PackageNameEnum.PRO.GetDisplayName())
                    {
                        roleId = (int)Helpers.Enum.PackageNameEnum.PRO;
                    }
                    else if(package.Name == Helpers.Enum.PackageNameEnum.PRO_PLUS.GetDisplayName())
                    {
                        roleId = (int)Helpers.Enum.PackageNameEnum.PRO_PLUS;
                    }
                    var role = await _unitOfWork.Repository<Role>().GetAll()
                        .Where(r => r.Id == roleId)
                        .FirstOrDefaultAsync();
                    if(role == null)
                    {
                        throw new ErrorResponse(404,
                            (int)RoleErrorEnum.NOT_FOUND,
                            RoleErrorEnum.NOT_FOUND.GetDisplayName());
                    }
                    account.RoleId = roleId;
                    account.Role = role;
                    await _unitOfWork.Repository<PremiumNote>().InsertAsync(premiumNote);
                    await _unitOfWork.Repository<Account>().UpdateDetached(account);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<BuyPackageResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<BuyPackageResponse>(premiumNote)
                    };
                }
                
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<FeatureResponse>> updateFeature(FeatureRequest request, int featureId)
        {
            try
            {
                var feature = await _unitOfWork.Repository<Feature>().GetAll()
                    .Where(f => f.Id == featureId)
                    .FirstOrDefaultAsync();
                if(feature == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.FEATURE_NOT_FOUND,
                        PackageErrorEnum.FEATURE_NOT_FOUND.GetDisplayName());
                }
                feature.Name = request.Name;
                feature.Description = request.Description;
                await _unitOfWork.Repository<Feature>().UpdateDetached(feature);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FeatureResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FeatureResponse>(feature)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<FeatureResponse>> changeFeatureStatus(int featureId, bool status)
        {
            try
            {
                var feature = await _unitOfWork.Repository<Feature>().GetAll()
                    .Where(f => f.Id == featureId)
                    .FirstOrDefaultAsync();
                if (feature == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.FEATURE_NOT_FOUND,
                        PackageErrorEnum.FEATURE_NOT_FOUND.GetDisplayName());
                }
                feature.IsActive = status;
                await _unitOfWork.Repository<Feature>().UpdateDetached(feature);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FeatureResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FeatureResponse>(feature)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<FeatureResponse>> deleteFeatureInDatabase(int featureId)
        {
            try
            {
                var feature = await _unitOfWork.Repository<Feature>().GetAll()
                    .Where(f => f.Id == featureId)
                    .FirstOrDefaultAsync();
                if (feature == null)
                {
                    throw new ErrorResponse(404,
                        (int)PackageErrorEnum.FEATURE_NOT_FOUND,
                        PackageErrorEnum.FEATURE_NOT_FOUND.GetDisplayName());
                }
                feature.IsActive = false;
                _unitOfWork.Repository<Feature>().Delete(feature);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FeatureResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FeatureResponse>(feature)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PackageResponse>> createPackage(PackageRequest request)
        {
            try
            {
                var package = _mapper.Map<Package>(request);
                package.IsActive = true;
                package.CreatedAt = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                await _unitOfWork.Repository<Package>().InsertAsync(package);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<PackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<PackageResponse>(package)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<FeatureResponse>> createFeature(FeatureRequest request)
        {
            try
            {
                var feature = _mapper.Map<Feature>(request);
                feature.IsActive = true;
                await _unitOfWork.Repository<Feature>().InsertAsync(feature);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FeatureResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FeatureResponse>(feature)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        //check Id trong featureIdList de loc cai nao ko co -> bao loi: success
        //dua 1 list moi se xoa hoan toan list cu de update
        public async Task<BaseResponseViewModel<PackageResponse>> addFeaturesToPackage(int packageId, List<int> featureIdList)
        {
            try
            {
                var packageList = await _unitOfWork.Repository<Package>().GetAll().ToListAsync();
                var package =  packageList
                            .Where(p => p.Id == packageId)
                            .FirstOrDefault();
                PackageResponse response = await getPackageById(packageId);
                if (package == null)
                {
                    throw new ErrorResponse(404, (int)PackageErrorEnum.PACKAGE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_NOT_FOUND.GetDisplayName());
                }
                if(featureIdList.Count > 0)
                {
                    featureIdList = featureIdList.Distinct().OrderBy(id => id).ToList();
                    var allFeature = await _unitOfWork.Repository<Feature>().GetAll().ToListAsync();
                    var allFeatureIds = allFeature.Select(f => f.Id).ToList();
                    var packageFeature = await _unitOfWork.Repository<PackageFeature>().GetAll().ToListAsync();
                    var existingPackageFeatureIds = packageFeature
                        .Where(pf => pf.PackageId == packageId)
                        .Select(pf => pf.FeatureId)
                        .ToList();
                    // Check if all feature IDs in featureIdList exist in the database
                    var missingFeatureIds = featureIdList.Except(allFeatureIds).ToList();

                    var featureIdsChange = featureIdList.SequenceEqual(existingPackageFeatureIds.Cast<int>());

                    if (missingFeatureIds.Any())
                    {
                        // Some feature IDs in featureIdList do not exist in the database
                        throw new ErrorResponse(
                            400, // You can choose an appropriate HTTP status code
                            (int)PackageErrorEnum.FEATURE_NOT_FOUND,
                            $"Some feature IDs do not exist: {string.Join(", ", missingFeatureIds)}");
                    }


                    if (featureIdsChange)
                    {
                        return new BaseResponseViewModel<PackageResponse>()
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
                    var packageFeatureToDelete = packageFeature
                        .Where(pf => pf.PackageId == packageId)
                        .ToList();
                    foreach(var pf in packageFeatureToDelete)
                    {
                        _unitOfWork.Repository<PackageFeature>().Delete(pf);
                    }

                    var featureIdListNullable = featureIdList.Select(id => (int?)id).ToList();
                    //var existingPackageFeatureIdsNullable = existingPackageFeatureIds.ToList();

                    var featuresToAdd = allFeature
                        //.Where(f => featureIdListNullable.Except(existingPackageFeatureIdsNullable).Contains(f.Id))
                        .Where(f => featureIdListNullable.Contains(f.Id))
                        .ToList();
                    if(featuresToAdd.Any(f => f.IsActive == false))
                    {
                        throw new ErrorResponse(400,
                            (int)PackageErrorEnum.INACTIVE_FEATURE,
                            PackageErrorEnum.INACTIVE_FEATURE.GetDisplayName() );
                    }
                    var packageFeatures = featuresToAdd.Select(feature => new PackageFeature
                    {
                        Feature = feature,
                        Package = package,
                        PackageId = packageId,
                        FeatureId = feature.Id
                    }).ToList();
                    foreach (var feature in packageFeatures)
                    {
                        await _unitOfWork.Repository<PackageFeature>().InsertAsync(feature);
                    }
                }
                await _unitOfWork.CommitAsync();
                response = await getPackageById(packageId);
                return new BaseResponseViewModel<PackageResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<PackageResponse>> getAllPackages(PagingRequest paging)
        {
            try
            {
                var packageList = _unitOfWork.Repository<Package>().GetAll()
                    .Include(p => p.PackageFeatures)
                    .ThenInclude(pf => pf.Feature)
                    .ProjectTo<PackageResponse>(_mapper.ConfigurationProvider)
                    .ToList();
                var response = packageList
                    .AsQueryable()
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<PackageResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = response.Item1
                    },
                    Data = response.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<FeatureResponse>> getAllFeatures(PagingRequest paging)
        {
            try
            {
                var featureList = _unitOfWork.Repository<Feature>().GetAll()
                    .Where(p => p.IsActive == true)
                    .ProjectTo<FeatureResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<FeatureResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = featureList.Item1
                    },
                    Data = featureList.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<PackageResponse>> createConfig(ConfigRequest request, int featureId, int packageId)
        {
            try
            {
                var packageFeature = await _unitOfWork.Repository<PackageFeature>()
                    .GetAll()
                    .Where(pf => pf.FeatureId == featureId && pf.PackageId == packageId)
                    .FirstOrDefaultAsync();
                if(packageFeature == null)
                {
                    throw new ErrorResponse(404, (int)PackageErrorEnum.PACKAGE_FEATURE_NOT_FOUND,
                        PackageErrorEnum.PACKAGE_FEATURE_NOT_FOUND.GetDisplayName());
                }    
                packageFeature.Number = request.Number;
                packageFeature.Config = request.ConfigName;
                packageFeature.Unit = request.Unit;
                await _unitOfWork.Repository<PackageFeature>().UpdateDetached(packageFeature);
                await _unitOfWork.CommitAsync();

                var response = await getPackageById(packageId);
                return new BaseResponseViewModel<PackageResponse>()
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
        private async Task<PackageResponse> getPackageById(int packageId)
        {
            try
            {
                // Fetch the Package entity by packageId including its related features
                var package = await _unitOfWork.Repository<Package>().GetAll()
                    .Include(p => p.PackageFeatures)
                    .ThenInclude(pf => pf.Feature)
                    .Where(p => p.Id == packageId)
                    .FirstOrDefaultAsync();

                // Check if the package exists
                if (package != null)
                {
                    // Use AutoMapper to map the Package entity to PackageResponse
                    var packageResponse = _mapper.Map<PackageResponse>(package);

                    // Optionally, you can map the Features of the package to FeatureResponse here

                    return packageResponse;
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
        private async Task<PackageResponse> checkBoughtOrNot(PackageResponse package, Guid accountId)
        {
            try
            {
                var check = await _unitOfWork.Repository<PremiumNote>().GetAll()
                    .Where(pn => pn.PackageId == package.Id && pn.AccountId == accountId && pn.IsActive == true)
                    .FirstOrDefaultAsync();
                package.IsBought = false;
                if (check != null)
                {
                    package.EndDate = check.EndDate;
                    package.IsBought = true;
                }
                return package;
            }
            catch (ErrorResponse ex)
            {
                // Handle ErrorResponses
                // Log the error or return an appropriate response
                throw;
            }
        }
        private async Task<bool> checkPremiumNoteExisted(int packageId, Guid accountId)
        {
            try
            {
                var check = _unitOfWork.Repository<PremiumNote>().GetAll()
                    .Any(pn => pn.PackageId == packageId && pn.AccountId == accountId && pn.IsActive == true);
                return check;
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
    }
}
