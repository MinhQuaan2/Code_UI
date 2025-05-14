using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.RequestResponse
{
    public class RequestFulfillmentResponse
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;

        public decimal Reward { get; set; }
        public Guid CreateBy { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterEmail { get; set; }
        public Guid? ReceiveBy { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverEmail { get; set; }
        public double Deposit { get; set; }
        public int Deadline { get; set; }
        public FulfillResponse? FulfillmentResponse { get; set; }
    }
    public class FulfillResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
