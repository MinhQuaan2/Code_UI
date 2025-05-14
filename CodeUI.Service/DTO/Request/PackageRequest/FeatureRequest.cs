using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.PackageRequest
{
    public class FeatureRequest
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
