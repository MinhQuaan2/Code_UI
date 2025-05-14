using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ReportRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.DTO.Response.RequestResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.NetworkInformation;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service.ModeratorServices
{
    public interface IModeratorReportService
    {
        Task<BaseResponsePagingViewModel<ModFulfillmentReportResponse>> getAllFulfillmentReport(PagingRequest paging, ReportStatusFilterRequest filter);
        Task<BaseResponseViewModel<ReportResultResponse>> rejectFulfillmentReport(int id, ResponseRequest request);
        Task<BaseResponseViewModel<ReportResultResponse>> approveFulfillmentReport(int id);
        Task<BaseResponsePagingViewModel<ModElementReportResponse>> getPendingElementReport(PagingRequest paging);
        Task<BaseResponseViewModel<ReportResultResponse>> approveElementReport(int id);
        Task<BaseResponseViewModel<ReportResultResponse>> rejectReport(int id, ResponseRequest request);
        Task<BaseResponsePagingViewModel<ReportResponse>> getByElementId(int id, PagingRequest paging, ReportStatusFilterRequest filter);
        Task<BaseResponsePagingViewModel<ModAccountReportResponse>> getPendingAccountReport(PagingRequest paging);
        Task<BaseResponseViewModel<ReportResultResponse>> approveAccountReport(int id);
        Task<BaseResponseViewModel<BlockedAccountResponse>> blockAccount(string ID);
        Task<BaseResponseViewModel<BlockedAccountResponse>> unblockAccount(string ID);
    }
    public class ModeratorReportService : IModeratorReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public ModeratorReportService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        public async Task<BaseResponsePagingViewModel<ModFulfillmentReportResponse>> getAllFulfillmentReport(PagingRequest paging, ReportStatusFilterRequest filter)
        {
            try
            {
                var reports = _unitOfWork.Repository<FulfillmentReport>().GetAll()
                    .Where(r => r.Status == filter.Status.ToString())
                    .AsQueryable();
                //.ProjectTo<ModFulfillmentReportResponse>(_mapper.ConfigurationProvider)
                //.PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                //if (filter.Status == ReStatusEnum.ALL)
                //{
                //    reports = _unitOfWork.Repository<FulfillmentReport>().GetAll()
                //        .AsQueryable();
                //}
                switch (filter.SortDate)
                {
                    case SortingOption.Ascending:
                        reports = reports.OrderBy(r => r.Timestamp);
                        break;
                    case SortingOption.Descending:
                        reports = reports.OrderByDescending(r => r.Timestamp);
                        break;
                    default:
                        reports = reports.OrderBy(r => r.Timestamp);
                        break;
                }
                var response = _mapper.Map<List<ModFulfillmentReportResponse>>(reports);
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = response.Count // Update this with the correct count
                    },
                    response.ToList()
                );
                //string filePath = "D:\\output.txt"; // Path to your fil
                //string textToWrite = sw0 + "\n" + sw1;

                //File.WriteAllText(filePath, textToWrite);
                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<ModFulfillmentReportResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<ReportResultResponse>> approveFulfillmentReport(int id)
        {
            try
            {
                var report = await _unitOfWork.Repository<FulfillmentReport>().GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportErrorEnum.NOT_FOUND,
                        ReportErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == report.Fulfillment.RequestId)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404, (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                       RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                var reporter = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == report.ReporterId)
                    .FirstOrDefaultAsync();
                if (reporter == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                if (existedRequest.Status == RequestStatusEnum.AVAILABLE.ToString())
                {
                    report.Status = ReportStatusEnum.APPROVED.ToString();
                    report.Response = AcceptReportResponseEnum.SUITABLE_FULFILLMENT.GetDisplayName();
                    existedRequest.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                    existedRequest.Status = RequestStatusEnum.BANNED.ToString();
                    reporter.Profile.Wallet += existedRequest.Reward;
                    PointsTransaction reporterTrans = new PointsTransaction()
                    {
                        Account = reporter,
                        AccountId = reporter.Id,
                        Amount = existedRequest.Reward,
                        Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                        Type = PointTransactionTypeEnum.REWARD_COMPLETION.GetDisplayName()
                    };
                    await _unitOfWork.Repository<Account>().UpdateDetached(reporter);
                    await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                    await _unitOfWork.Repository<PointsTransaction>().InsertAsync(reporterTrans);
                    await _unitOfWork.Repository<FulfillmentReport>().UpdateDetached(report);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<ReportResultResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<ReportResultResponse>(report)
                    };
                }
                else
                {
                    report.Status = ReportStatusEnum.SEND_TO_ADMIN.ToString();
                    await _unitOfWork.Repository<FulfillmentReport>().UpdateDetached(report);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<ReportResultResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<ReportResultResponse>(report)
                    };
                }
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<ReportResultResponse>> rejectFulfillmentReport(int id, ResponseRequest request)
        {
            try
            {
                var report = await _unitOfWork.Repository<FulfillmentReport>().GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportErrorEnum.NOT_FOUND,
                        ReportErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.Status = ReportStatusEnum.REJECTED.ToString();
                report.Response = request.response;
                await _unitOfWork.Repository<FulfillmentReport>().UpdateDetached(report);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<ReportResultResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResultResponse>(report)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<ReportResultResponse>> approveAccountReport(int id)
        {
            try
            {
                var reports = _unitOfWork.Repository<Report>().GetAll();
                var report = await reports
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportErrorEnum.NOT_FOUND,
                        ReportErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Where(r => r.Id == report.TargetAccountId)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                //var similarReports = await reports
                //    .Where(r => r.TargetAccountId == report.TargetAccountId && r.Status == Helpers.Enum.ReportStatusEnum.PENDING.ToString())
                //    .ToListAsync();
                //similarReports.ForEach(async r =>
                //{
                //    r.Status = Helpers.Enum.ReportStatusEnum.SOLVED.ToString();
                //    r.Response = Helpers.Enum.AcceptReportEnum.SKIPPED.GetDisplayName().ToString();
                //    await _unitOfWork.Repository<Report>().UpdateDetached(r);
                //});
                report.Status = Helpers.Enum.ReportStatusEnum.PROCESS.ToString();
                report.Response = Helpers.Enum.AcceptReportEnum.SEND_TO_ADMIN.GetDisplayName().ToString();
                await _unitOfWork.Repository<Report>().UpdateDetached(report);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<ReportResultResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResultResponse>(report)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<ReportResultResponse>> approveElementReport(int id)
        {
            try
            {
                var reports = _unitOfWork.Repository<Report>().GetAll();
                var report = await reports
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportErrorEnum.NOT_FOUND,
                        ReportErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var element = await _unitOfWork.Repository<Element>()
                    .GetAll()
                    .Where(r => r.Id == report.ElementId)
                    .FirstOrDefaultAsync();
                if (element == null)
                {
                    throw new ErrorResponse(404, (int)ElementErrorEnum.NOT_FOUND,
                        ElementErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var similarReports = await reports
                    .Where(r => r.ElementId == report.ElementId && r.Status == Helpers.Enum.ReportStatusEnum.PENDING.ToString())
                    .ToListAsync();
                similarReports.ForEach(async r =>
                {
                    r.Status = Helpers.Enum.ReportStatusEnum.SOLVED.ToString();
                    r.Response = Helpers.Enum.AcceptReportEnum.SKIPPED.GetDisplayName().ToString();
                    await _unitOfWork.Repository<Report>().UpdateDetached(r);
                });
                report.Status = Helpers.Enum.ReportStatusEnum.APPROVED.ToString();
                report.Response = Helpers.Enum.AcceptReportEnum.ACCEPT_REPORT.GetDisplayName().ToString();
                await _unitOfWork.Repository<Report>().UpdateDetached(report);

                
                element.IsActive = false;
                await _unitOfWork.Repository<Element>().UpdateDetached(element);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<ReportResultResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResultResponse>(report)
                };
            }catch(ErrorResponse ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<ReportResultResponse>> rejectReport(int id, ResponseRequest request)
        {
            try
            {
                var report = await _unitOfWork.Repository<Report>()
                    .GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportErrorEnum.NOT_FOUND,
                        ReportErrorEnum.NOT_FOUND.GetDisplayName());
                }
                report.Status = Helpers.Enum.ReportStatusEnum.REJECTED.ToString();
                report.Response = request.response;
                await _unitOfWork.Repository<Report>().UpdateDetached(report);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<ReportResultResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportResultResponse>(report)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<ModElementReportResponse>> getPendingElementReport(PagingRequest paging)
        {
            try
            {
                //var stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start();

                var reportsList = _unitOfWork.Repository<Element>()
                    .GetAll()
                    .Where(r => r.Reports.Any(r => r.Status == Helpers.Enum.ReportStatusEnum.PENDING.ToString()))
                    .ToList();
                //stopwatch.Stop();
                //string sw0 = $"Begin Time: {stopwatch.ElapsedMilliseconds} milliseconds";

                //stopwatch.Restart(); // Restart the stopwatch for the main processing
                //var distinctReports = reportsList
                //    .DistinctBy(r => r.ElementId);
                //    //.ToList();
                
                var response = _mapper.Map<List<ModElementReportResponse>>(reportsList);
                //stopwatch.Stop();
                //string sw1 = $"After Time: {stopwatch.ElapsedMilliseconds} milliseconds";

                //stopwatch.Restart(); // Restart the stopwatch for the main processing
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = response.Count // Update this with the correct count
                    },
                    response.ToList()
                );
                //string filePath = "D:\\output.txt"; // Path to your fil
                //string textToWrite = sw0 + "\n" + sw1;

                //File.WriteAllText(filePath, textToWrite);
                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<ModElementReportResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
                
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<ReportResponse>> getByElementId(int id, PagingRequest paging, ReportStatusFilterRequest filter)
        {
            try
            {
                var reports = _unitOfWork.Repository<Report>().GetAll();
                //if(filter.)
                var reportList = reports
                    .Where(r => r.ElementId == id && r.Status == Helpers.Enum.ReportStatusEnum.PENDING.ToString())
                    .OrderBy(r => r.Timestamp);
                var result = reportList
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


        public async Task<BaseResponsePagingViewModel<ModAccountReportResponse>> getPendingAccountReport(PagingRequest paging)
        {
            var reportsList = _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Where(r => r.ReportTargetAccounts.Any(r => r.Status == Helpers.Enum.ReportStatusEnum.PENDING.ToString()))
                    .ToList();
            var response = _mapper.Map<List<ModAccountReportResponse>>(reportsList);
            var result = (
                new PagingsMetadata
                {
                    Page = paging.Page,
                    Size = paging.PageSize,
                },
                response.ToList()
            );
            return new BaseResponsePagingViewModel<ModAccountReportResponse>()
            {
                Metadata = result.Item1,
                Data = result.Item2
            };
        }

        public async Task<BaseResponseViewModel<BlockedAccountResponse>> blockAccount(string ID)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID));
                if(account == null)
                {
                    throw new ErrorResponse(404, 4041, "Account not found!");
                }
                else if(account.IsActive == false)
                {
                    throw new ErrorResponse(400, 4001, "Account is already blocked!");
                }
                account.IsActive = false;
                account.UpdateDate = DateTime.Now;

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<BlockedAccountResponse>()
                {
                    Data = new BlockedAccountResponse()
                    {
                        Id = account.Id,
                        Username = account.Username,
                        Email = account.Email,
                        IsActive = account.IsActive,
                        IsBlocked = !account.IsActive
                    },
                    Status = new StatusViewModel()
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    }
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        
        public async Task<BaseResponseViewModel<BlockedAccountResponse>> unblockAccount(string ID)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID));
                if(account == null)
                {
                    throw new ErrorResponse(404, 4041, "Account not found!");
                }
                else if(account.IsActive != false)
                {
                    throw new ErrorResponse(400, 4002, "Account is already unblocked!");
                }
                account.IsActive = true;
                account.UpdateDate = DateTime.Now;

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<BlockedAccountResponse>()
                {
                    Data = new BlockedAccountResponse()
                    {
                        Id = account.Id,
                        Username = account.Username,
                        Email = account.Email,
                        IsActive = account.IsActive,
                        IsBlocked = !account.IsActive
                    },
                    Status = new StatusViewModel()
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    }
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
