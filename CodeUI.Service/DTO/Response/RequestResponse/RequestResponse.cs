using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.RequestResponse
{
    public class RequestResponse
    {
        public int Id { get; set; }

        public string? RequestDescription { get; set; }

        public string? Status { get; set; }

        public decimal Reward { get; set; }
        public Guid CreateBy { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterEmail { get; set; }
        public Guid? ReceiveBy { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverEmail { get; set; }
        public string? Name { get; set; }

        public double Deposit { get; set; }
        public DateTime StartDate { get; set; }
        public int Deadline { get; set; }

        public string? ImageUrl1 { get; set; }

        public string? ImageUrl2 { get; set; }

        public string? ImageUrl3 { get; set; }
        public string? CategoryName { get; set; }
        public string? Avatar { get; set; }
        public string? TypeCss { get; set; }
        public bool isAccepted { get; set; } = false;
    }
}
