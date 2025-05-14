using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.PaymentRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PaymentResponse;
using CodeUI.Service.DTO.Response.PaymentResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Service;
using Google.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace CodeUI.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }
        [HttpPost("createPayment")]
        public async Task<ActionResult<BaseResponseViewModel<PaymentResponse>>> CreatePayment([FromBody][Required] PaymentRequest request)
        {
            try
            {
                var result = await _paymentService.createPayment(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirmPayment")]
        public async Task<ActionResult<BaseResponseViewModel<PaymentConfirmResponse>>> ConfirmPayment()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountID == null)
                {
                    return Unauthorized();
                }
                if (Request.Query.Count > 0)
                {
                    string hashSecret = _configuration["VNPayConfig:HashSecret"]; // Retrieve the hash secret from configuration
                    var vnpayData = Request.Query;
                    PayLib pay = new PayLib();
                    //test change server
                    // Get all query string parameters starting with "vnp_"
                    foreach (var kvp in vnpayData)
                    {
                        if (!string.IsNullOrEmpty(kvp.Key) && kvp.Key.StartsWith("vnp_"))
                        {
                            pay.AddResponseData(kvp.Key, kvp.Value);
                        }
                    }

                    PaymentConfirmRequest request = new PaymentConfirmRequest();
                    request.orderId = pay.GetResponseData("vnp_TxnRef"); //mã hóa đơn
                    request.vnpayTranId = pay.GetResponseData("vnp_TransactionNo"); //mã giao dịch tại hệ thống VNPAY
                    request.vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); // Response code: 00 - success, other codes - see documentation
                    request.vnp_SecureHash = Request.Query["vnp_SecureHash"]; // Hash of the returned data
                    request.amount = Convert.ToInt64(pay.GetResponseData("vnp_Amount"));
                    request.date = pay.GetResponseData("vnp_PayDate");
                    request.orderInfo = pay.GetResponseData("vnp_OrderInfo");
                    request.vnp_BankCode = pay.GetResponseData("vnp_BankCode");

                    request.checkSignature = pay.ValidateSignature(request.vnp_SecureHash, hashSecret); // Check if the signature is valid
                    var result = await _paymentService.confirmPayment(request, Guid.Parse(accountID));
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Internal server error occurred.");
                }
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("getPaymentHistory")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PaymentHistoryResponse>>> GetPaymentHistory([FromQuery]PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountID == null)
                {
                    return Unauthorized();
                }
                var result = await _paymentService.getPaymentHistory(Guid.Parse(accountID), paging);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        [HttpGet("getTransactionHistory")]
        public async Task<ActionResult<BaseResponsePagingViewModel<TransactionResponse>>> GetTransactionHistory([FromQuery]PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountID = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountID == null)
                {
                    return Unauthorized();
                }
                var result = await _paymentService.getTransactionHistory(Guid.Parse(accountID), paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
    }
}
