using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Resource;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.AccountRequest;
using CodeUI.Service.DTO.Request.ProfileRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using FirebaseAdmin.Auth;
using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IAccountService
    {
        Task<BaseResponseViewModel<AccountResponse>> GetAccountByToken(string accountId);
        Task<BaseResponsePagingViewModel<MultipleAccountResponse>> GetAccounts(MultipleAccountResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request);
        Task<BaseResponseViewModel<LoginResponse>> LoginByMail(ExternalAuthRequest request);
        Task<BaseResponsePagingViewModel<TopAccountResponse>> GetTopAccountByElementCount(int amount, PagingRequest paging);
    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request)
        {
            try
            { 
                #region Create and Map Account

                var newAccount = _mapper.Map<Account>(request);
                newAccount.RoleId = (int)SystemRoleTypeEnum.FreeCreator;
                newAccount.IsActive = true;
                newAccount.CreateDate = DateTime.Now;
                newAccount.Profile.Account =  newAccount;
                newAccount.Profile.Wallet = 0;

                #endregion

                await _unitOfWork.Repository<Account>().InsertAsync(newAccount);

                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(newAccount)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<LoginResponse>> LoginByMail(ExternalAuthRequest request)
        {
            try
            {
                string newAccessToken = null;

                //decode token -> user record
                var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
                FirebaseToken decodeToken = await auth.VerifyIdTokenAsync(request.IdToken);
                UserRecord userRecord = await auth.GetUserAsync(decodeToken.Uid);

                //check exist customer 
                var account = _unitOfWork.Repository<Account>().GetAll()
                                .FirstOrDefault(x => x.Email.Contains(userRecord.Email));

                //new customer => add fcm map with Id
                if (account is null)
                {
                    CreateAccountRequest newAccountRequest = new CreateAccountRequest()
                    {
                        ID = Guid.NewGuid(),
                        Username = userRecord.Email.Split("@").FirstOrDefault() + '_' + Utils.GenerateRandomCode(6),
                        Email = userRecord.Email,
                        Phone = userRecord.PhoneNumber,
                        CreateProfile = new CreateProfileRequest
                        {
                            ImageUrl = (userRecord.PhotoUrl == null || userRecord.PhotoUrl == "") ? "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_960_720.png" : userRecord.PhotoUrl
                        }
                    };

                    //create account
                    var createResult = CreateAccount(newAccountRequest).Result.Data;
                    account = _mapper.Map<Account>(createResult);


                }

                var result = _mapper.Map<AccountResponse>(account);
                result.ProfileResponse.Username = account.Username;
                var role = result.Role == null ? "FreeCreator" : result.Role;

                newAccessToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Username) ? "" : account.Username,
                                                                      new string[] { role } ,
                                                                      account.Id, _configuration);

                return new BaseResponseViewModel<LoginResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = new LoginResponse()
                    {
                        access_token = newAccessToken,
                        account = result
                    }
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<MultipleAccountResponse>> GetAccounts(MultipleAccountResponse filter, PagingRequest paging)
        {
            try
            {
                var customer = _unitOfWork.Repository<Account>().GetAll()
                    .ProjectTo<MultipleAccountResponse>(_mapper.ConfigurationProvider)
                    .DynamicFilter(filter)
                    .DynamicSort(filter)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<MultipleAccountResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = customer.Item1
                    },
                    Data = customer.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> GetAccountByToken(string accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(x => x.Id == Guid.Parse(accountId))
                    .FirstOrDefaultAsync();

                if (account == null)
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<TopAccountResponse>> GetTopAccountByElementCount(int amount, PagingRequest paging)
        {
            try
            {
                var elements = _unitOfWork.Repository<Element>().GetAll();
                var accounts = _unitOfWork.Repository<Account>().GetAll();
                var topAccounts = elements.GroupBy(x => x.OwnerId)
                                         .Select(group => new TopAccountResponse()
                                         {
                                             AccountId = group.Key,
                                             Username = accounts.FirstOrDefault(x => x.Id == group.Key).Username,
                                             ImageUrl = accounts.FirstOrDefault(x => x.Id == group.Key).Profile.ImageUrl,
                                             ElementCount = group.Count()
                                         })
                                         .OrderByDescending(x => x.ElementCount)
                                         .Take(amount)
                                         .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<TopAccountResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = topAccounts.Item1
                    },
                    Data = topAccounts.Item2.ToList()
                };
            }
            catch(ErrorResponse ex) 
            {
                throw ex; 
            }
        }

    }
}


