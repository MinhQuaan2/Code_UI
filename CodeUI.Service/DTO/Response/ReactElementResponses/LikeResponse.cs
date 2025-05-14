using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReactElementResponses
{
    public class LikeResponse
    {
        public DateTime? Timestamp { get; set; }
        public string? Action { get; set; }
        public int? ElementId { get; set; }
        public Guid? AccountId { get; set; }
    }
}
