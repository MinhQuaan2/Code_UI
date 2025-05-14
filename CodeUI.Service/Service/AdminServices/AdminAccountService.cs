using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace CodeUI.Service.Service.AdminServices
{
    public interface IAdminAccountService
    {
        Task<BaseResponsePagingViewModel<AdminAccountResponse>> GetAllAccounts(AdminAccountResponse filter,PagingRequest paging);
        Task<BaseResponsePagingViewModel<AccountResponse>> GetInactiveAccounts(AccountResponse filter,PagingRequest paging);
        Task<DashboardNewAccountResponse> GetNewAccounts(int month,int year);
        Task<BaseResponseViewModel<DashboardAccountResponse>> GetDashboardAccount(int month, int year);
    }
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminAccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponsePagingViewModel<AdminAccountResponse>> GetAllAccounts(AdminAccountResponse filter,PagingRequest paging)
        {
            try
            {
                var accounts = _unitOfWork.Repository<Account>().GetAll()
                                                                .ProjectTo<AdminAccountResponse>(_mapper.ConfigurationProvider)
                                                                .DynamicFilter(filter)
                                                                .DynamicSort(filter)
                                                                .PagingQueryable(paging.Page,paging.PageSize,Constants.LimitPaging,Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<AdminAccountResponse>
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accounts.Item1
                    },
                    Data = accounts.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<DashboardNewAccountResponse> GetNewAccounts(int month, int year)
        {
            try
            {
                var accounts = _unitOfWork.Repository<Account>().GetAll();
                var newAccountsAmount = accounts.Where(x => x.CreateDate.Value.Month == month && x.CreateDate.Value.Year == year).Count();
                var lastMonthAccountsAmount = accounts.Where(x => x.CreateDate.Value.Month == (month - 1) && x.CreateDate.Value.Year == year).Count();

                return new DashboardNewAccountResponse
                {
                    Month = month,
                    TotalNewAccount = newAccountsAmount,
                    Diff= newAccountsAmount - lastMonthAccountsAmount
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountResponse>> GetInactiveAccounts(AccountResponse filter, PagingRequest paging)
        {
            try
            {
                var accounts = _unitOfWork.Repository<Account>().GetAll()
                                                                .Where(x => x.IsActive == false)
                                                                .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
                                                                .DynamicFilter(filter)
                                                                .DynamicSort(filter)
                                                                .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<AccountResponse>
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accounts.Item1
                    },
                    Data = accounts.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<DashboardAccountResponse>> GetDashboardAccount(int month,int year)
        {
            try
            {
                //if(month > DateTime.Now.Month)
                //{
                //    throw new ErrorResponse(400, 4001, "The month is invalid!");
                //}
                var accounts = _unitOfWork.Repository<Account>().GetAll();
                var totalAccounts = accounts.Where(x => x.CreateDate.Value.Month == month && x.CreateDate.Value.Year == year).Count();
                var accountDiff = accounts.Where(x => x.CreateDate.Value.Month == month - 1 && x.CreateDate.Value.Year == year).Count();

                return new BaseResponseViewModel<DashboardAccountResponse>
                {
                    Status = new StatusViewModel
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    },
                    Data = new DashboardAccountResponse
                    {
                        Month = month,
                        AccountDiff = totalAccounts - accountDiff,
                        TotalAccount = totalAccounts
                    }
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
