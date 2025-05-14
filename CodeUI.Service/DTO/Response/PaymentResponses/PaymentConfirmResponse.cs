using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PaymentResponse
{
    public class PaymentConfirmResponse
    {
        public bool Status;
        public long Amount;
        public string? VnPayId;
        public string? OrderId;
        public string? OrderInfo;
        public string? Date;
    }
}
