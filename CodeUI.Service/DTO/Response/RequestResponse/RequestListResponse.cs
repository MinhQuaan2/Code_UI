using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.RequestResponse
{
    public class RequestListResponse
    {
        public int Id { get; set; }
        public string? RequestDescription { get; set; }
        public string? Status { get; set; }
        public string? Avatar { get; set; }
        public string? Name { get; set; }
        public string? CategoryName { get; set; }
        public decimal Reward { get; set; }
        public int Deadline { get; set; }
        public DateTime StartDate { get; set; }
        public Guid requesterId { get; set; }
        public string? TypeCss { get; set; }
    }
}
