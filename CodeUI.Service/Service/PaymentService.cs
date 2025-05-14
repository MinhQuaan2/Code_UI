using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Request.PaymentRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.DonationResponse;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response.PaymentResponse;
using CodeUI.Service.DTO.Response.PaymentResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static CodeUI.Service.Helpers.ErrorEnum;
using static Google.Rpc.Context.AttributeContext.Types;

namespace CodeUI.Service.Service
{
    public interface IPaymentService
    {
        Task<BaseResponseViewModel<PaymentResponse>> createPayment(PaymentRequest paymentRequest);
        Task<BaseResponsePagingViewModel<PaymentMethodResponse>> getPaymentMethod();
        Task<BaseResponsePagingViewModel<UpdatePackageResponse>> getPaymentPackage();
        Task<BaseResponseViewModel<PaymentConfirmResponse>> confirmPayment(PaymentConfirmRequest request, Guid accountId);
        Task<BaseResponsePagingViewModel<PaymentHistoryResponse>> getPaymentHistory(Guid accountId, PagingRequest paging);
        Task<BaseResponsePagingViewModel<TransactionResponse>> getTransactionHistory(Guid accountId, PagingRequest paging);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<BaseResponseViewModel<PaymentResponse>> createPayment(PaymentRequest request)
        {
            try
            {
                var vNPayConfig = _configuration.GetSection("VNPayConfig");
                string url = vNPayConfig["Url"];
                //string returnUrl = vNPayConfig["ReturnUrl"];
                string tmnCode = vNPayConfig["TmnCode"];
                string hashSecret = vNPayConfig["HashSecret"];

                PayLib pay = new PayLib();
                Util util = new Util();
                string ipAddress = util.GetIpAddress();

                pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
                pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                pay.AddRequestData("vnp_Amount", (request.Money*100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                pay.AddRequestData("vnp_IpAddr", ipAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch
                pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                pay.AddRequestData("vnp_OrderInfo", request.OrderDescription); //Thông tin mô tả nội dung thanh toán
                pay.AddRequestData("vnp_OrderType", request.OrderType); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                pay.AddRequestData("vnp_ReturnUrl", request.returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

                PaymentResponse paymentResponse = new PaymentResponse();
                paymentResponse.Status = "successful";
                paymentResponse.paymentUrl = paymentUrl;
                paymentResponse.Message = "ok";

                return new BaseResponseViewModel<PaymentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = paymentResponse
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponsePagingViewModel<PaymentMethodResponse>> getPaymentMethod()
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponsePagingViewModel<UpdatePackageResponse>> getPaymentPackage()
        {
            throw new NotImplementedException();
        }
        public async Task<BaseResponseViewModel<PaymentConfirmResponse>> confirmPayment(PaymentConfirmRequest request, Guid accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == accountId)
                    .FirstOrDefaultAsync();
                if(account == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var check = _unitOfWork.Repository<Transaction>().GetAll()
                    .Any(t => t.VnPayId == request.vnpayTranId && t.OrderId == request.orderId);
                if(check == true)
                {
                    throw new ErrorResponse(400,
                        (int)PaymentErrorEnum.TRANSACTION_DUPLICATE,
                        PaymentErrorEnum.TRANSACTION_DUPLICATE.GetDisplayName());
                }
                PaymentConfirmResponse response = new PaymentConfirmResponse();
                if (request.checkSignature)
                {
                    if (request.vnp_ResponseCode == "00")
                    {
                        Transaction trans = new Transaction()
                        {
                            Amount = request.amount/100,
                            Account = account,
                            AccountId = accountId,
                            Date = DateTime.ParseExact(request.date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            Status = true,
                            OrderId = request.orderId,
                            OrderInfo = request.orderInfo,
                            VnPayId = request.vnpayTranId,
                            ResponseCode = request.vnp_ResponseCode,
                            PaymentMethod = request.vnp_BankCode
                        };
                        if(account.Profile.Wallet == null)
                        {
                            account.Profile.Wallet = 0;
                        }
                        account.Profile.Wallet += request.amount / 100;
                        await _unitOfWork.Repository<Account>().UpdateDetached(account);
                        await _unitOfWork.Repository<Transaction>().InsertAsync(trans);
                        await _unitOfWork.CommitAsync();
                        response = _mapper.Map<Transaction, PaymentConfirmResponse>(trans);
                    }
                    else
                    {
                        Transaction trans = new Transaction()
                        {
                            Amount = request.amount/100,
                            Account = account,
                            AccountId = accountId,
                            Date = DateTime.ParseExact(request.date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            Status = false,
                            OrderId = request.orderId,
                            OrderInfo = request.orderInfo,
                            VnPayId = request.vnpayTranId,
                            ResponseCode = request.vnp_ResponseCode,
                            PaymentMethod = request.vnp_BankCode
                        };
                        await _unitOfWork.Repository<Transaction>().InsertAsync(trans);
                        await _unitOfWork.CommitAsync();
                    }
                }
                else
                {
                    throw new ErrorResponse(400, (int)PaymentErrorEnum.ERROR_OCCURRED, PaymentErrorEnum.ERROR_OCCURRED.GetDisplayName());
                }
                    
                return new BaseResponseViewModel<PaymentConfirmResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
                // Handle the rest of your payment confirmation logic
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<PaymentHistoryResponse>> getPaymentHistory(Guid accountId, PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == accountId)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var response = _unitOfWork.Repository<Transaction>().GetAll()
                    .Where(p => p.AccountId == accountId)
                    .OrderByDescending(p => p.Date) // Use OrderByDescending for newest to oldest
                    .ProjectTo<PaymentHistoryResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<PaymentHistoryResponse>()
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
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<TransactionResponse>> getTransactionHistory(Guid accountId, PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => a.Id == accountId)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404,
                        (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var response = _unitOfWork.Repository<PointsTransaction>().GetAll()
                    .Where(p => p.AccountId == accountId)
                    .OrderByDescending(p => p.Timestamp) // Use OrderByDescending for newest to oldest
                    .ProjectTo<TransactionResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<TransactionResponse>()
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
    }
}
