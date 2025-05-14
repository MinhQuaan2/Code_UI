using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ReportRequest
{
    public class ReportRequest
    {
        [Required]
        public string? ReportContent { get; set; }
        public List<ReportImageRequest>? ReportImages { get; set; }
    }
    public class ReportImageRequest
    {
        [Required]
        public string? ImageUrl { get; set; }
    }
}
