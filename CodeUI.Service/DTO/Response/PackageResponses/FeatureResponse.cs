using CodeUI.Service.DTO.Response.PackageResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PackageResponse
{
    public class FeatureResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public ConfigResponse? Config { get; set; }
    }
}
