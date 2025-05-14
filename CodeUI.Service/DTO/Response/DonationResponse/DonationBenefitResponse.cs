using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.DonationResponse
{
    public class DonationBenefitResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }
    }
}
