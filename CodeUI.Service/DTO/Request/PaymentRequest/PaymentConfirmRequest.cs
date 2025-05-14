using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.PaymentRequest
{
    public class PaymentConfirmRequest
    {
        public string? orderId { get; set; }
        public string? vnpayTranId { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_SecureHash { get; set; }
        public long amount { get; set; }
        public string? date { get; set; }
        public string? orderInfo { get; set; }
        public bool checkSignature { get; set; }
        public string? vnp_BankCode { get; set; }
    }
}
