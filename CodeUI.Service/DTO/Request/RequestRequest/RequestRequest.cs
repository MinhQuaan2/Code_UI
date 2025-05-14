using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace CodeUI.Service.DTO.Request.RequestRequest
{
    public class RequestRequest
    {
        public required string RequestDescription { get; set; }
        [Range(10000, 10000000, ErrorMessage = "Money must be greater than 10.000 and smaller than 10.000.000")]
        [DefaultValue(0)]
        public required decimal Reward { get; set; }
        public required string Name { get; set; }
        public required int Deadline { get; set; }
        public string? ImageUrl1 { get; set; }

        public string? ImageUrl2 { get; set; }

        public string? ImageUrl3 { get; set; }
        public required string? Avatar { get; set; }
        public required string? CategoryName { get; set; }
        public string? TypeCss { get; set; }
    }
}
