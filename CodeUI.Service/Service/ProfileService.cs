using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Request.ProfileRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ProfileResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IProfileService
    {
        Task<BaseResponseViewModel<ProfileResponse>> UpdateProfile(string accountId, UpdateProfileRequest request);

        Task<BaseResponseViewModel<ProfileResponse>> GetProfileByAccountId(string accountId);
        Task<BaseResponseViewModel<ProfileResponse>> GetProfileByUsername(string accountID, string username);
    }
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProfileService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseViewModel<ProfileResponse>> UpdateProfile(string accountId, UpdateProfileRequest request)
        {
            try
            {
                var profile = _unitOfWork.Repository<Data.Entity.Profile>().GetAll().FirstOrDefault(x => x.Account.Id == Guid.Parse(accountId) );
                var accounts = _unitOfWork.Repository<Account>().GetAll();
                if(profile == null)
                {
                    throw new ErrorResponse(404, (int)ProfileErrorEnum.NOT_FOUND, ProfileErrorEnum.NOT_FOUND.GetDisplayName());
                }

                #region Validate update profile request
                if (request.Username == null)
                {
                    request.Username = profile.Account.Username;
                }
                else if (!request.Username.Equals("string") && !request.Username.Equals("") && request.Username != null)
                {
                    var checkAccountDuplicate = accounts.FirstOrDefault(x => x.Username == request.Username && x.Id != Guid.Parse(accountId));
                    var account = accounts.FirstOrDefault(x => x.Id == Guid.Parse(accountId));
                    if(checkAccountDuplicate != null)
                    {
                        throw new ErrorResponse(400, (int)ProfileErrorEnum.USERNAME_ALREADY_EXIST, ProfileErrorEnum.USERNAME_ALREADY_EXIST.GetDisplayName());
                    }
                    if ((DateTime.Now - account.UpdateDate) <= TimeSpan.FromDays(30))
                    {
                        throw new ErrorResponse(400, (int)ProfileErrorEnum.UPDATED_RECENTLY, ProfileErrorEnum.UPDATED_RECENTLY.GetDisplayName());
                    }
                    account.Username = request.Username;
                    account.UpdateDate = DateTime.Now;
                }

                if(request.DateOfBirth < (DateTime.Now - TimeSpan.FromDays(365 * 100)) ||
                   request.DateOfBirth > DateTime.Now - TimeSpan.FromDays(365 * 6))
                {
                    throw new ErrorResponse(400, (int)ProfileErrorEnum.INVALID_DOB, 
                        ProfileErrorEnum.INVALID_DOB.GetDisplayName());
                }

                if(request.Wallet < 0)
                {
                    throw new ErrorResponse(400, (int)ProfileErrorEnum.NEGATIVE_WALLET, 
                        ProfileErrorEnum.NEGATIVE_WALLET.GetDisplayName());
                }

                if(Regex.IsMatch(request.Phone, RegexEnum.Phone.GetDisplayName()))
                {
                    throw new ErrorResponse(400, (int)ProfileErrorEnum.INVALID_PHONE_NUMBER, 
                        ProfileErrorEnum.INVALID_PHONE_NUMBER.GetDisplayName());
                }

                

                #endregion

                var updatedProfile = _mapper.Map<UpdateProfileRequest,Data.Entity.Profile>(request,profile);

                await _unitOfWork.Repository<Data.Entity.Profile>().UpdateDetached(updatedProfile);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ProfileResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ProfileResponse>(updatedProfile)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<ProfileResponse>> GetProfileByAccountId(string accountId)
        {
            try
            {
                var profile = await _unitOfWork.Repository<Data.Entity.Profile>().GetAll().FirstOrDefaultAsync(x => x.Account.Id == Guid.Parse(accountId));

                if (profile == null)
                {
                    throw new ErrorResponse(400, (int)ProfileErrorEnum.NOT_FOUND, 
                        ProfileErrorEnum.NOT_FOUND.GetDisplayName());
                }

                var result = _mapper.Map<ProfileResponse>(profile);

                result.Username = profile.Account.Username;
                result.AccountID = profile.Account.Id;

                return new BaseResponseViewModel<ProfileResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = result
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        
        public async Task<BaseResponseViewModel<ProfileResponse>> GetProfileByUsername(string accountID, string username)
        {
            try
            {
                var profiles = _unitOfWork.Repository<Data.Entity.Profile>().GetAll();
                var profile = await profiles.FirstOrDefaultAsync(x => x.Account.Username == username);
                if (profile == null)
                {
                    throw new ErrorResponse(400, (int)ProfileErrorEnum.NOT_FOUND,
                        ProfileErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var result = _mapper.Map<ProfileResponse>(profile);
                if (accountID != null)
                {
                    var myself = await profiles.FirstOrDefaultAsync(x => x.Account.Id == Guid.Parse(accountID));

                    //if(profile.Account.FollowFollowers.Any(x => x.FollowerId == Guid.Parse(accountID) && x.FollowId == profile.Account.Id))
                    if (myself.Account.FollowFollowers.Any(x => x.FollowerId == Guid.Parse(accountID) && x.FollowId == profile.Account.Id))
                    {
                        result.IsFollow = true;
                    }
                }
                result.Username = profile.Account.Username;
                result.AccountID = profile.Account.Id;

                return new BaseResponseViewModel<ProfileResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = result
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
