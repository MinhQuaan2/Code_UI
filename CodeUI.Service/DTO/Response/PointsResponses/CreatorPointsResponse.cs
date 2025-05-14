using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PointsResponses
{
    public class CreatorPointsResponse
    {
        public Guid AccountId { get; set; }

        public string? Username { get; set; }

        public DateTime TransactionDate { get; set; }

        public int Amount { get; set; }

        public string Type { get; set; } = null!;
    }
}
