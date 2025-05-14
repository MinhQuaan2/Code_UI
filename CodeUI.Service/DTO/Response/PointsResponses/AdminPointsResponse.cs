using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PointsResponses
{
    public class AdminPointsResponse
    {
        public DateTime Timestamp { get; set; }

        public Guid AccountId { get; set; }
        public string Username { get; set; }

        public string Type { get; set; } = null!;

        public decimal Amount { get; set; }

    }
}
