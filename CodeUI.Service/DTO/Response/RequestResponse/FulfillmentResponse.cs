using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.RequestResponse
{
    public class FulfillmentResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? Status { get; set; }
        public int RequestId { get; set; }
        public string? RequesterEmail { get; set; }
    }
}
