using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.RequestRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.DTO.Response.PaymentResponses;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.DTO.Response.RequestResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using Google.Apis.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IRequestService
    {
        Task<BaseResponseViewModel<RequestResponse>> createRequest(Guid creator, RequestRequest request);
        Task<BaseResponseViewModel<RequestResponse>> updateRequest(int requestId, UpdateRequest request, Guid creatorId);
        Task<BaseResponseViewModel<RequestResponse>> cancelRequest(int requestId, Guid creatorId);
        Task<BaseResponsePagingViewModel<RequestListResponse>> getRequestList(PagingRequest paging, SortAndFilterRequest request, string accountId, string userId);
        Task<BaseResponseViewModel<RequestResponse>> getRequestById(int id, string accountId);
        Task<BaseResponseViewModel<RequestFulfillmentResponse>> acceptRequest(int id, Guid recipientId);
        Task<BaseResponseViewModel<RequestResponse>> giveUpRequest(int id, Guid recipientId);
        Task<BaseResponsePagingViewModel<FulfillmentResponse>> getFulfillmentByRequestId(int requestId, Guid creatorId, PagingRequest paging);
        Task<BaseResponsePagingViewModel<FulfillmentResponse>> getProcessFulfillment(Guid recipientId, PagingRequest paging, int requestId);
        Task<BaseResponsePagingViewModel<FulfillmentResponse>> getOwnFulfillment(Guid recipientId, PagingRequest paging);
        Task<BaseResponsePagingViewModel<FulfillmentResponse>> getOwnFulfillmentByRequestId(Guid recipientId, PagingRequest paging, int requestId);
        Task<BaseResponseViewModel<FulfillmentResponse>> submitFulfillment(Guid recipientId, int fulFillmentId);
        Task<BaseResponseViewModel<RequestResponse>> AcceptFulfillment(int fulFillmentId, Guid requesterId);
        Task<BaseResponseViewModel<RequestResponse>> RejectFulfillment(int fulFillmentId, Guid requesterId, string reason);
        Task<BaseResponsePagingViewModel<RequestListResponse>> getOwnRequestList(PagingRequest paging, SortAndFilterRequest request, Guid creatorId);
        Task<BaseResponseViewModel<RequestResponse>> deleteRequestInDatabase(int requestId);
        Task<BaseResponseViewModel<FulfillmentDetailResponse>> getFulfillmentDetailById(int Id);
    }
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public RequestService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        public async Task<BaseResponseViewModel<RequestResponse>> deleteRequestInDatabase(int requestId)
        {
            try
            {
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == requestId)
                    .FirstOrDefaultAsync();
                if(existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                _unitOfWork.Repository<Request>().Delete(existedRequest);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(existedRequest)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> createRequest(Guid creatorId, RequestRequest request)
        {
            try
            {
                var newRequest = _mapper.Map<Request>(request);
                if(newRequest == null)
                {
                    throw new ErrorResponse(500,
                        (int)RequestErrorEnum.CREATE_FAILED,
                        RequestErrorEnum.CREATE_FAILED.GetDisplayName());
                }
                newRequest.StartDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                newRequest.Status = Helpers.Enum.RequestStatusEnum.AVAILABLE.ToString();
                newRequest.Deposit = 0;
                var creator = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == creatorId)
                    .FirstOrDefaultAsync();
                if(creator == null)
                {
                    throw new ErrorResponse(404,
                       (int)AccountErrorEnum.NOT_FOUND,
                       AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                if (creator.Profile.Wallet.GetValueOrDefault() < request.Reward)
                {
                    throw new ErrorResponse(400,
                       (int)RequestErrorEnum.LACK_MONEY,
                       RequestErrorEnum.LACK_MONEY.GetDisplayName());
                }
                var category = await _unitOfWork.Repository<Category>().GetAll() 
                    .Where(c => c.Name == request.CategoryName)
                    .FirstOrDefaultAsync();
                if(category == null)
                {
                    throw new ErrorResponse(404, 
                        (int)CategoryErrorEnum.NOT_FOUND
                        ,CategoryErrorEnum.NOT_FOUND.GetDisplayName());
                }
                newRequest.Category = category;
                newRequest.CategoryId = category.Id;
                newRequest.CreateBy = creatorId;
                newRequest.CreateByNavigation = creator;
                creator.Profile.Wallet -= request.Reward;
                PointsTransaction transaction = new PointsTransaction()
                {
                    Account = creator,
                    AccountId = creatorId,
                    Amount = -request.Reward,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Type = PointTransactionTypeEnum.CREATE_REQUEST.GetDisplayName()
                };
                await _unitOfWork.Repository<PointsTransaction>().InsertAsync(transaction);
                await _unitOfWork.Repository<Request>().InsertAsync(newRequest);
                await _unitOfWork.Repository<Account>().UpdateDetached(creator);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(newRequest)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> updateRequest(int requestId, UpdateRequest request, Guid creatorId)
        {
            try
            {
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == requestId)
                    .FirstOrDefaultAsync();
                if(creatorId != existedRequest.CreateBy)
                {
                    throw new ErrorResponse(403,
                        (int)RequestErrorEnum.FORBIDDEN,
                        RequestErrorEnum.FORBIDDEN.GetDisplayName());
                }
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                if (existedRequest.ReceiveBy != null || existedRequest.Status == Helpers.Enum.RequestStatusEnum.PROCESSING.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_AVAILABLE,
                        RequestErrorEnum.NOT_AVAILABLE.GetDisplayName());
                }
                existedRequest.ImageUrl1 = request.ImageUrl1;
                existedRequest.ImageUrl2 = request.ImageUrl2;
                existedRequest.ImageUrl3 = request.ImageUrl3;
                existedRequest.RequestDescription = request.RequestDescription;
                existedRequest.Name = request.Name;
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(existedRequest)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> cancelRequest(int requestId, Guid creatorId)
        {
            try
            {
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == requestId)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                var creator = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == creatorId)
                    .FirstOrDefaultAsync();
                if (creator == null)
                {
                    throw new ErrorResponse(404,
                       (int)AccountErrorEnum.NOT_FOUND,
                       AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                if (creatorId != existedRequest.CreateBy)
                {
                    throw new ErrorResponse(403,
                        (int)RequestErrorEnum.FORBIDDEN,
                        RequestErrorEnum.FORBIDDEN.GetDisplayName());
                }
                if (existedRequest.ReceiveBy != null || existedRequest.Status != Helpers.Enum.RequestStatusEnum.AVAILABLE.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_AVAILABLE,
                        RequestErrorEnum.NOT_AVAILABLE.GetDisplayName());
                }
                existedRequest.Status = Helpers.Enum.RequestStatusEnum.CANCELED.ToString();
                existedRequest.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                creator.Profile.Wallet += (decimal)0.95 * existedRequest.Reward;
                PointsTransaction transaction = new PointsTransaction()
                {
                    Account = creator,
                    AccountId = creatorId,
                    Amount = -(decimal)0.95 * existedRequest.Reward,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Type = PointTransactionTypeEnum.CANCEL_REQUEST.GetDisplayName()
                };
                await _unitOfWork.Repository<PointsTransaction>().InsertAsync(transaction);
                AdminPoint point = new AdminPoint()
                {
                    Account = creator,
                    AccountId = creatorId,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Amount = (decimal)0.95 * existedRequest.Reward + existedRequest.Deposit,
                    Type = PointTransactionTypeEnum.CANCEL_REQUEST.GetDisplayName()
                };
                await _unitOfWork.Repository<AdminPoint>().InsertAsync(point);
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(existedRequest)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<RequestListResponse>> getRequestList(PagingRequest paging, SortAndFilterRequest request, string accountId, string userId)
        {
            try
            {
                var query = _unitOfWork.Repository<Request>().GetAll()
                        .Where(r => r.Status == request.Status.ToString())
                        .AsQueryable();
                if(request.Status == RequestStatusEnum.ALL)
                {
                    query = _unitOfWork.Repository<Request>().GetAll()
                        .AsQueryable();
                }
                if(accountId != null)
                {
                    query = query
                        .Where(r => r.CreateBy == Guid.Parse(accountId));
                }
                if(userId != null)
                {
                    query = query
                        .Where(r => r.CreateBy != Guid.Parse(userId));
                }
                // Sorting
                switch (request.SortReward)
                {
                    case SortingOption.Ascending:
                        query = query.OrderBy(r => r.Reward);
                        break;
                    case SortingOption.Descending:
                        query = query.OrderByDescending(r => r.Reward);
                        break;
                    default:
                        break;
                }

                switch (request.SortStartDate)
                {
                    case SortingOption.Ascending:
                        query = query.OrderBy(r => r.StartDate);
                        break;
                    case SortingOption.Descending:
                        query = query.OrderByDescending(r => r.StartDate);
                        break;
                    default:
                        break;  
                }

                // Filtering
                if (!string.IsNullOrEmpty(request.CategoryFilter))
                {
                    query = query.Where(r => r.Category.Name == request.CategoryFilter);
                }
                if (!string.IsNullOrEmpty(request.CreatorName))
                {
                    query = query.Where(r => r.CreateByNavigation.Username == request.CreatorName);
                }
                if (!string.IsNullOrEmpty(request.RequestName))
                {
                    query = query.Where(r => r.Name == request.RequestName);
                }
                var response = query
                    .ProjectTo<RequestListResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<RequestListResponse>()
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
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<RequestListResponse>> getOwnRequestList(PagingRequest paging, SortAndFilterRequest request, Guid creatorId)
        {
            try
            {
                var query = _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Status == request.Status.ToString() && r.CreateBy == creatorId)
                    .AsQueryable();

                // Sorting
                switch (request.SortReward)
                {
                    case SortingOption.Ascending:
                        query = query.OrderBy(r => r.Reward);
                        break;
                    case SortingOption.Descending:
                        query = query.OrderByDescending(r => r.Reward);
                        break;
                    default:
                        break;
                }

                switch (request.SortStartDate)
                {
                    case SortingOption.Ascending:
                        query = query.OrderBy(r => r.StartDate);
                        break;
                    case SortingOption.Descending:
                        query = query.OrderByDescending(r => r.StartDate);
                        break;
                    default:
                        break;
                }

                // Filtering
                if (!string.IsNullOrEmpty(request.CategoryFilter))
                {
                    query = query.Where(r => r.Category.Name == request.CategoryFilter);
                }
                if (!string.IsNullOrEmpty(request.CreatorName))
                {
                    query = query.Where(r => r.CreateByNavigation.Username == request.CreatorName);
                }

                var response = query
                    .ProjectTo<RequestListResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<RequestListResponse>()
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
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> getRequestById(int id, string accountId)
        {
            try
            {
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                var response = _mapper.Map<RequestResponse>(existedRequest);
                if (accountId != null)
                {
                    var check = await checkIsAccepted(id, accountId);
                    response.isAccepted = check;
                }
                
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(response)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestFulfillmentResponse>> acceptRequest(int id, Guid recipientId)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == recipientId)
                    .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if(existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                if(existedRequest.Status != Helpers.Enum.RequestStatusEnum.AVAILABLE.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_AVAILABLE,
                        RequestErrorEnum.NOT_AVAILABLE.GetDisplayName());
                }
                if(existedRequest.CreateBy == recipientId)
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.RECIPIENT_REQUEST,
                        RequestErrorEnum.RECIPIENT_REQUEST.GetDisplayName());
                }
                var fee = existedRequest.Reward * 10 / 100;
                if (recipient.Profile.Wallet < fee)
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.LACK_MONEY,
                        RequestErrorEnum.LACK_MONEY.GetDisplayName());
                }
                existedRequest.Deposit += fee;
                existedRequest.Status = RequestStatusEnum.PROCESSING.ToString();
                existedRequest.ReceiveBy = recipientId;
                existedRequest.ReceiveByNavigation = recipient;
                recipient.Profile.Wallet -= fee;
                PointsTransaction transaction = new PointsTransaction()
                {
                    Account = recipient,
                    AccountId = recipientId,
                    Amount = -fee,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Type = PointTransactionTypeEnum.ACCEPT_REQUEST_FEE.GetDisplayName()
                };
                var fulfillment = _mapper.Map<Fulfillment>(existedRequest);
                fulfillment.StartDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                fulfillment.Owner = recipient;
                fulfillment.OwnerId = recipientId;
                fulfillment.IsActive = true;
                fulfillment.Status = FulfillmentStatusEnum.PROCESSING.ToString();
                fulfillment.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now.AddHours(existedRequest.Deadline), _timeZoneInfo);
                existedRequest.Fulfillments.Add(fulfillment);
                await _unitOfWork.Repository<PointsTransaction>().InsertAsync(transaction);
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.Repository<Account>().UpdateDetached(recipient);
                await _unitOfWork.CommitAsync();
                var response = _mapper.Map<RequestFulfillmentResponse>(existedRequest);
                var fulfillmentToResponse = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Status == FulfillmentStatusEnum.PROCESSING.ToString() && f.RequestId == id && f.IsActive == true && f.OwnerId == recipientId)
                    .FirstOrDefaultAsync();
                if (fulfillmentToResponse == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                var fulfillmentResponse = _mapper.Map<FulfillResponse>(fulfillmentToResponse);
                response.FulfillmentResponse = fulfillmentResponse;
                return new BaseResponseViewModel<RequestFulfillmentResponse>()
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
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> giveUpRequest(int id, Guid recipientId)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == recipientId)
                    .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                if (existedRequest.Status != RequestStatusEnum.PROCESSING.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_AVAILABLE,
                        RequestErrorEnum.NOT_AVAILABLE.GetDisplayName());
                }
                if (existedRequest.CreateBy == recipientId)
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.RECIPIENT_REQUEST,
                        RequestErrorEnum.RECIPIENT_REQUEST.GetDisplayName());
                }
                if(existedRequest.ReceiveBy !=  recipientId)
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_RECIPIENT,
                        RequestErrorEnum.NOT_RECIPIENT.GetDisplayName());
                }
                existedRequest.Status = RequestStatusEnum.AVAILABLE.ToString();
                existedRequest.ReceiveBy = null;
                existedRequest.ReceiveByNavigation = null;
                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.OwnerId == recipientId && f.IsActive == true)
                    .FirstOrDefaultAsync();
                if(fulfillment == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                fulfillment.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                fulfillment.IsActive = false;
                fulfillment.Status = FulfillmentStatusEnum.CANCELED.ToString();
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.Repository<Fulfillment>().UpdateDetached(fulfillment);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RequestResponse>(existedRequest)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<FulfillmentResponse>> getFulfillmentByRequestId(int requestId, Guid creatorId, PagingRequest paging)
        {
            try
            {
                var creator = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == creatorId)
                    .FirstOrDefaultAsync();
                if (creator == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == requestId)
                    .FirstOrDefaultAsync();
                if (creatorId != existedRequest.CreateBy)
                {
                    throw new ErrorResponse(403,
                        (int)RequestErrorEnum.FORBIDDEN,
                        RequestErrorEnum.FORBIDDEN.GetDisplayName());
                }
                var response = _mapper.Map<List<FulfillmentResponse>>(existedRequest.Fulfillments.ToList());
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = response.Count // Update this with the correct count
                    },
                    response.ToList()
                );
                return new BaseResponsePagingViewModel<FulfillmentResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<FulfillmentResponse>> getProcessFulfillment(Guid recipientId, PagingRequest paging, int requestId)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == recipientId)
                    .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var fulfillmentList = _unitOfWork.Repository<Fulfillment>().GetAll()
                        .Where(f => f.IsActive == true && f.OwnerId == recipientId)
                        .OrderBy(x => x.EndDate)
                        .ProjectTo<FulfillmentResponse>(_mapper.ConfigurationProvider)
                        .ToList();
                //.PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                if (requestId != 0)
                {
                    var response = fulfillmentList
                        .Where(f => f.RequestId == requestId)
                        .AsQueryable()
                        .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                    return new BaseResponsePagingViewModel<FulfillmentResponse>()
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
                else
                {
                    var response = fulfillmentList
                            .AsQueryable()
                            .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                    return new BaseResponsePagingViewModel<FulfillmentResponse>()
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
                
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponsePagingViewModel<FulfillmentResponse>> getOwnFulfillment(Guid recipientId, PagingRequest paging)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == recipientId)
                    .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var response = _unitOfWork.Repository<Fulfillment>().GetAll()
                        .Where(f => f.OwnerId == recipientId)
                        .OrderBy(x => x.StartDate)
                        .ProjectTo<FulfillmentResponse>(_mapper.ConfigurationProvider)
                        .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<FulfillmentResponse>()
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
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<FulfillmentDetailResponse>> getFulfillmentDetailById(int Id)
        {
            try
            {
                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Id == Id)
                    .FirstOrDefaultAsync();
                return new BaseResponseViewModel<FulfillmentDetailResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FulfillmentDetailResponse>(fulfillment)
                };
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
       
        public async Task<BaseResponsePagingViewModel<FulfillmentResponse>> getOwnFulfillmentByRequestId(Guid recipientId, PagingRequest paging, int requestId)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == recipientId)
                    .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var response = _unitOfWork.Repository<Fulfillment>().GetAll()
                        .Where(f => f.OwnerId == recipientId && f.RequestId == requestId)
                        .OrderBy(x => x.StartDate)
                        .ProjectTo<FulfillmentResponse>(_mapper.ConfigurationProvider)
                        .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<FulfillmentResponse>()
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
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<FulfillmentResponse>> submitFulfillment(Guid recipientId, int fulFillmentId)
        {
            try
            {
                var recipient = await _unitOfWork.Repository<Account>().GetAll()
                        .Where(a => a.Id == recipientId)
                        .FirstOrDefaultAsync();
                if (recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Id == fulFillmentId && f.IsActive == true)
                    .FirstOrDefaultAsync();
                if (fulfillment == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                fulfillment.Status = RequestStatusEnum.COMPLETED.ToString();
                if (fulfillment.OwnerId != recipientId)
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.NOT_RECIPIENT,
                        RequestErrorEnum.NOT_RECIPIENT.GetDisplayName());
                }
                fulfillment.Request.Status = RequestStatusEnum.SUBMITTED.ToString();
                await _unitOfWork.Repository<Fulfillment>().UpdateDetached(fulfillment);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<FulfillmentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<FulfillmentResponse>(fulfillment)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> AcceptFulfillment(int fulFillmentId, Guid requesterId)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll().AsQueryable();
                var requester = await account
                    .Where(a => a.Id == requesterId)
                    .FirstOrDefaultAsync();

                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Id == fulFillmentId && f.IsActive == true)
                    .FirstOrDefaultAsync();
                if (fulfillment == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                if (fulfillment.Status != FulfillmentStatusEnum.COMPLETED.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.FULFILLEMNT_NOT_COMPLETE,
                        RequestErrorEnum.FULFILLEMNT_NOT_COMPLETE.GetDisplayName());
                }
                if (fulfillment.Request.CreateBy != requesterId)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.CONFUSION_FULFILLMENT,
                        RequestErrorEnum.CONFUSION_FULFILLMENT.GetDisplayName());
                }

                var recipient = await account
                    .Where(a => a.Id == fulfillment.OwnerId && a.IsActive == true)
                    .FirstOrDefaultAsync();
                if (requester == null || recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                
                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == fulfillment.RequestId)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                
                existedRequest.Status = RequestStatusEnum.COMPLETED.ToString();
                existedRequest.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                fulfillment.Status = FulfillmentStatusEnum.APPROVED.ToString();
                requester.Profile.Wallet += existedRequest.Deposit;
                recipient.Profile.Wallet += existedRequest.Reward;
                PointsTransaction requesterTrans = new PointsTransaction()
                {
                    Account = requester,
                    AccountId = requesterId,
                    Amount = existedRequest.Deposit,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Type = PointTransactionTypeEnum.DEPOSIT_COMPENSATION.GetDisplayName()
                };
                PointsTransaction recipientTrans = new PointsTransaction()
                {
                    Account = recipient,
                    AccountId = recipient.Id,
                    Amount = existedRequest.Reward,
                    Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    Type = PointTransactionTypeEnum.REWARD_COMPLETION.GetDisplayName()
                };
                await _unitOfWork.Repository<Account>().UpdateDetached(requester);
                await _unitOfWork.Repository<Account>().UpdateDetached(recipient);
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.Repository<Fulfillment>().UpdateDetached(fulfillment);
                await _unitOfWork.Repository<PointsTransaction>().InsertAsync(requesterTrans);
                await _unitOfWork.Repository<PointsTransaction>().InsertAsync(recipientTrans);
                await _unitOfWork.CommitAsync();
                var response = _mapper.Map<RequestResponse>(existedRequest);
                response.RequesterEmail = existedRequest.CreateByNavigation.Email;
                response.ReceiverEmail = existedRequest.ReceiveByNavigation.Email;
                return new BaseResponseViewModel<RequestResponse>()
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
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<RequestResponse>> RejectFulfillment(int fulFillmentId, Guid requesterId, string reason)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll().AsQueryable();
                var requester = await account
                    .Where(a => a.Id == requesterId)
                    .FirstOrDefaultAsync();

                var fulfillment = await _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Where(f => f.Id == fulFillmentId && f.IsActive == true)
                    .FirstOrDefaultAsync();
                if (fulfillment == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.FULFILLMENT_NOT_FOUND,
                        RequestErrorEnum.FULFILLMENT_NOT_FOUND.GetDisplayName());
                }
                if (fulfillment.Status != FulfillmentStatusEnum.COMPLETED.ToString())
                {
                    throw new ErrorResponse(400,
                        (int)RequestErrorEnum.FULFILLEMNT_NOT_COMPLETE,
                        RequestErrorEnum.FULFILLEMNT_NOT_COMPLETE.GetDisplayName());
                }
                if (fulfillment.Request.CreateBy != requesterId)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.CONFUSION_FULFILLMENT,
                        RequestErrorEnum.CONFUSION_FULFILLMENT.GetDisplayName());
                }

                var recipient = await account
                    .Where(a => a.Id == fulfillment.OwnerId && a.IsActive == true)
                    .FirstOrDefaultAsync();
                if (requester == null || recipient == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }

                var existedRequest = await _unitOfWork.Repository<Request>().GetAll()
                    .Where(r => r.Id == fulfillment.RequestId)
                    .FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    throw new ErrorResponse(404,
                        (int)RequestErrorEnum.REQUEST_NOT_FOUND,
                        RequestErrorEnum.REQUEST_NOT_FOUND.GetDisplayName());
                }
                
                fulfillment.Status = FulfillmentStatusEnum.REJECTED.ToString();
                fulfillment.IsActive = false;
                fulfillment.EndDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                fulfillment.Reason = reason;
                
                var response = _mapper.Map<RequestResponse>(existedRequest);
                response.RequesterEmail = existedRequest.CreateByNavigation.Email;
                response.ReceiverEmail = existedRequest.ReceiveByNavigation.Email;
                existedRequest.Status = RequestStatusEnum.AVAILABLE.ToString();
                existedRequest.ReceiveBy = null;
                existedRequest.ReceiveByNavigation = null;
                await _unitOfWork.Repository<Request>().UpdateDetached(existedRequest);
                await _unitOfWork.Repository<Fulfillment>().UpdateDetached(fulfillment);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<RequestResponse>()
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
                throw ex;
            }
        }
        private async Task<bool> checkIsAccepted(int requestId, string accountId)
        {
                var response = _unitOfWork.Repository<Fulfillment>().GetAll()
                    .Any(f => f.RequestId == requestId && f.OwnerId == Guid.Parse(accountId));
                return response;
        }
    }
}
