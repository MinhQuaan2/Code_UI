using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.DonationRequest
{
    public class DonationPackageRequest
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int SubscribeLimit { get; set; } = -1;
        [Required]
        public DonationPackageDetailRequest? DetailRequest { get; set; }
    }
    public class DonationPackageDetailRequest
    {
        [Required]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
