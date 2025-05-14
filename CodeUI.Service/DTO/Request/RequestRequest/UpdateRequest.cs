using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.RequestRequest
{
    public class UpdateRequest
    {
        public required string RequestDescription { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl1 { get; set; }

        public string? ImageUrl2 { get; set; }

        public string? ImageUrl3 { get; set; }
    }
}
