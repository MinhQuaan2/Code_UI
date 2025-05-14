using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace CodeUI.Service.DTO.Request.PaymentRequest
{
    public class PaymentRequest
    {
        [Range(10000, 10000000, ErrorMessage = "Money must be greater than 10.000 and smaller than 10.000.000")]
        [DefaultValue(0)]
        public required int Money { get; set; }
        public required string OrderType { get; set; }
        public required string OrderDescription { get; set; }
        public required string returnUrl { get; set; }
    }
}
