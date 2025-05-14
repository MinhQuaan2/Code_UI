using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ElementRequest
{
    public class UpdateStatusRequest
    {
        public string ModID { get; set; }
        public Dictionary<int,string> ElementStatuses { get; set; }
    }
}
