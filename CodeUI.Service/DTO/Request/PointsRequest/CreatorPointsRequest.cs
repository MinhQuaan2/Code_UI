using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.PointsRequest
{
    public class CreatorPointsRequest
    {
        public Guid AccountId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Type { get; set; } = null!;
    }
}
