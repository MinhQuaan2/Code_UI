using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PackageResponse
{
    public class PackageResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsBought { get; set; } = false;
        public int TotalFeature { get; set; }
        public List<FeatureResponse>? Features { get; set; }
    }
}
