using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PaymentResponses
{
    public class TransactionResponse
    {
        public decimal Amount { get; set; }

        public string Type { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
