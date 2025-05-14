using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PackageResponses
{
    public class BuyPackageResponse
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public PackageBoughtResponse? Package { get; set; }
        public CustomerResponse? Customer { get; set; }
    }
    public class PackageBoughtResponse
    {
        public string? Name { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }
    }
    public class CustomerResponse
    {
        public string? Username { get; set; }
        public string? Role { get; set; }
    }
}
