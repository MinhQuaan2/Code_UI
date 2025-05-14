using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ReportRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IReportService
    {
        Task<BaseResponseViewModel<ReportResponse>> createElementReport(Guid reporterId, int elementId, ReportRequest request, Helpers.Enum.ReportElementReasonEnum reason);
        Task<BaseResponseViewModel<ReportResponse>> createAccountReport(Guid reporterId, string username, ReportRequest request, Helpers.Enum.ReportAccountReasonEnum reason);
        Task<BaseResponsePagingViewModel<ReportResponse>> getAllAccountReport(PagingRequest paging, GetReportRequest request, string username);
        Task<BaseResponsePagingViewModel<ReportResponse>> getAllElementReport(PagingRequest paging, GetReportRequest request, int elementId);
        Task<BaseResponseViewModel<FulfillmentReportResponse>> createFulfillmentReport(Guid reporterId, int fulfillmentId, FulfillmentReportRequest request);
        Task<BaseResponsePagingViewModel<FulfillmentReportResponse>> getOwnFulfillmentReport(PagingRequest paging, Guid accountId);
    }
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        public async Task<BaseResponsePagingViewModel<FulfillmentReportResponse>> getOwnFulfillmentReport(PagingRequest paging, Guid accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(r => r.Id == accountId)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var reports = _unitOfWork.Repository<FulfillmentReport>().GetAll();
                var filteredReports = reports.Where(r => r.ReporterId == accountId);
                var result = filteredReports
                    .ProjectTo<FulfillmentReportResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                var response = new BaseResponsePagingViewModel<FulfillmentReportResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };
                return response;
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<ReportResponse>> createElementReport(Guid reporterId, int elementId, ReportRequest request, Helpers.Enum.ReportElementReasonEnum reason)
        {
            try
            {
                var report = _mapper.Map<Report>(request);
                var account = _unitOfWork.Repository<Account>().GetAll();
                var reporter = await account.Where(r => r.Id == reporterId)
                            .FirstOrDefaultAsync();
                if (reporter == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.Reporter = reporter;
                report.ReporterId = reporterId;
                var targetElement = await _unitOfWork.Repository<Element>().GetAll()
                    .Where(e => e.Id == elementId && e.IsActive == true)
                    .FirstOrDefaultAsync();
                if (targetElement == null)
                {
                    throw new ErrorResponse(404, (int)ElementErrorEnum.NOT_FOUND,
                        ElementErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.Element = targetElement;
                report.ElementId = elementId;
                report.Type = Helpers.Enum.ReportTypeEnum.ELEMENT.ToString();
                report.Status = Helpers.Enum.ReportStatusEnum.PENDING.ToString();
                report.Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                report.Reason = reason.ToString();

                await _unitOfWork.Repository<Report>().InsertAsync(report);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ReportResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResponse>(report)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<ReportResponse>> createAccountReport(Guid reporterId, string username, ReportRequest request, Helpers.Enum.ReportAccountReasonEnum reason)
        {
            try
            {
                var report = _mapper.Map<Report>(request);
                var account = _unitOfWork.Repository<Account>().GetAll();
                var reporter = await account.Where(r => r.Id == reporterId)
                            .FirstOrDefaultAsync();
                if (reporter == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.Reporter = reporter;
                report.ReporterId = reporterId;

                var targetAccount = await account.Where(t => t.Username == username)
                            .FirstOrDefaultAsync();
                if (targetAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                    AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.TargetAccount = targetAccount;
                report.TargetAccountId = targetAccount.Id;
                report.Type = Helpers.Enum.ReportTypeEnum.ACCOUNT.ToString();

                report.Status = Helpers.Enum.ReportStatusEnum.PENDING.ToString();
                report.Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                report.Reason = reason.ToString();

                await _unitOfWork.Repository<Report>().InsertAsync(report);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ReportResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResponse>(report)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<ReportResponse>> getAllElementReport(PagingRequest paging, GetReportRequest request, int elementId)
        {
            try
            {
                var reports = _unitOfWork.Repository<Report>().GetAll();
                var filteredReports = reports.Where(r => r.Type == Helpers.Enum.ReportTypeEnum.ELEMENT.ToString());

                if (elementId != 0)
                {
                    filteredReports = filteredReports.Where(r => r.ElementId == elementId);
                }

                var result = filteredReports
                    .ProjectTo<ReportResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                var response = new BaseResponsePagingViewModel<ReportResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };

                return response;
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<ReportResponse>> getAllAccountReport(PagingRequest paging, GetReportRequest request, string username)
        {
            try
            {
                var reports = _unitOfWork.Repository<Report>().GetAll();
                var filteredReports = reports.Where(r => r.Type == Helpers.Enum.ReportTypeEnum.ACCOUNT.ToString());

                if (username != null)
                {
                    filteredReports = filteredReports.Where(r => r.Reporter.Username == username);
                }

                var result = filteredReports
                    .ProjectTo<ReportResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                var response = new BaseResponsePagingViewModel<ReportResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };

                return response;
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<FulfillmentReportResponse>> createFulfillmentReport(Guid reporterId, int fulfillmentId, FulfillmentReportRequest request)
        {
            try
            {
                var reporter = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == reporterId)
                    .FirstOrDefaultAsync();
                if(reporter  == null)
                {
                    throw new ErrorResponse(404, 
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Id == fulfillmentId)
                    .FirstOrDefaultAsync();
                if(fulfillment == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                var report = _mapper.Map<FulfillmentReport>(request);
                report.Status = Helpers.Enum.ReportStatusEnum.PENDING.ToString();
                report.Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                report.FulfillmentId = fulfillmentId;
                report.ReporterId= reporterId;
                report.Fulfillment = fulfillment;
                report.Reporter = reporter;
                await _unitOfWork.Repository<FulfillmentReport>().InsertAsync(report);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FulfillmentReportResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FulfillmentReportResponse>(report)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

    }
}
