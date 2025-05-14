using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PaymentResponses
{
    public class PaymentHistoryResponse
    {
        public string? Username { get; set; }
        public DateTime Date { get; set; }
        public string OrderInfo { get; set; } = null!;

        public string OrderId { get; set; } = null!;

        public string VnPayId { get; set; } = null!;

        public bool Status { get; set; }
        public long Amount { get; set; }
    }
}
