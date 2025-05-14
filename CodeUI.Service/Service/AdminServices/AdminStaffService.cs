using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.StaffRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.DTO.Response.StaffResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service.AdminServices
{
    public interface IAdminStaffService
    {
        Task<BaseResponseViewModel<StaffResponse>> CreateModAccount(CreateStaffRequest staff);
        Task<BaseResponsePagingViewModel<QueryStaffRequest>> GetAllStaffAccount(QueryStaffRequest staff, PagingRequest paging);
        Task<BaseResponseViewModel<QueryStaffRequest>> GetSingleStaffAccount(string ID);
        Task<BaseResponseViewModel<StaffResponse>> UpdateStaffInfo(string ID, UpdateStaffRequest request);
        Task DisableStaff(string ID);
    }
    public class AdminStaffService : IAdminStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminStaffService(IUnitOfWork unitOfWork, IMapper mapper,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<BaseResponseViewModel<StaffResponse>> CreateModAccount(CreateStaffRequest request)
        {
            try
            {
                var checkStaff = _unitOfWork.Repository<StaffAccount>().GetAll().FirstOrDefault(x => x.Username == request.Username);
                if(checkStaff != null)
                {
                    throw new ErrorResponse(400, (int)StaffErrorEnum.DUPLICATE,
                        StaffErrorEnum.DUPLICATE.GetDisplayName());
                }

                var staff = _mapper.Map<StaffAccount>(request);
                staff.Id = Guid.NewGuid();
                staff.IsActive = true;
                staff.RoleId = 4;
                staff.CreateDate= DateTime.Now;
                staff.UpdateDate = DateTime.Now;

                await _unitOfWork.Repository<StaffAccount>().InsertAsync(staff);
                await _unitOfWork.CommitAsync();

                string role = ((SystemRoleTypeEnum)staff.RoleId).ToString();

                string accessToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(staff.Username) ? "" : staff.Username,
                                                                      new string[] { role },
                                                                      staff.Id, _configuration);

                return new BaseResponseViewModel<StaffResponse>()
                {
                    Status = new StatusViewModel
                    {
                        Success = true,
                        Message = "Success!",
                        ErrorCode = 0
                    },
                    Data = new StaffResponse
                    {
                        Username = staff.Username,
                        Token = accessToken
                    }
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<StaffResponse>> UpdateStaffInfo(string ID, UpdateStaffRequest request)
        {
            try
            {
                var staff = _unitOfWork.Repository<StaffAccount>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID));

                if (staff == null)
                {
                    throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND,
                        StaffErrorEnum.NOT_FOUND.GetDisplayName());
                }

                staff.Fullname = request.Fullname;
                staff.Username = request.Username;
                staff.Email = request.Email;
                staff.Phone = request.Phone;
                staff.UpdateDate= DateTime.Now;

                await _unitOfWork.Repository<StaffAccount>().UpdateDetached(staff);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<StaffResponse>
                {
                    Status = new StatusViewModel()
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true,
                    },
                    Data = _mapper.Map<StaffResponse>(staff)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<QueryStaffRequest>> GetAllStaffAccount(QueryStaffRequest filter, PagingRequest paging)
        {
            try
            {
                var customer = _unitOfWork.Repository<StaffAccount>().GetAll()
                                                                     .Where(x => x.RoleId == 4 && x.IsActive == true)
                                                                     .ProjectTo<QueryStaffRequest>(_mapper.ConfigurationProvider)
                                                                     .DynamicFilter(filter)
                                                                     .DynamicSort(filter)
                                                                     .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<QueryStaffRequest>()
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

        public async Task<BaseResponseViewModel<QueryStaffRequest>> GetSingleStaffAccount(string ID)
        {
            try
            {
                var staff = _unitOfWork.Repository<StaffAccount>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID));

                if(staff == null)
                {
                    throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND,
                        StaffErrorEnum.NOT_FOUND.GetDisplayName());
                }
                if(staff.IsActive == false)
                {
                    throw new ErrorResponse(400,(int)StaffErrorEnum.INACTIVE,
                        StaffErrorEnum.INACTIVE.GetDisplayName());
                }

                return new BaseResponseViewModel<QueryStaffRequest>()
                {
                    Status = new StatusViewModel
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true,
                    },
                    Data = _mapper.Map<QueryStaffRequest>(staff)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task DisableStaff(string ID)
        {
            try
            {
                var staff = _unitOfWork.Repository<StaffAccount>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID));

                if (staff == null)
                {
                    throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND,
                        StaffErrorEnum.NOT_FOUND.GetDisplayName());
                }

                staff.IsActive = false;
                staff.UpdateDate = DateTime.Now;

                await _unitOfWork.Repository<StaffAccount>().UpdateDetached(staff);
                await _unitOfWork.CommitAsync();
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
