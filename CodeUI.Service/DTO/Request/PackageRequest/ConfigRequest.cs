using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.PackageRequest
{
    public class ConfigRequest
    {
        public string? ConfigName { get;set; }
        public int Number { get; set; }
        public string? Unit { get; set; }
    }
}
