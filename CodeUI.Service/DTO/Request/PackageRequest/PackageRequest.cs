using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace CodeUI.Service.DTO.Request.PackageRequest
{
    public class PackageRequest
    {
        public required string? Name { get; set; }
        [Range(10000, int.MaxValue, ErrorMessage = "Price must be greater than 10000.")]
        [DefaultValue(0)]
        public required decimal Price { get; set; }
        public required int Duration { get; set; }
    }
}
