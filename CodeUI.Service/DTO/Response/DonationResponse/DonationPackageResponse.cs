﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.DonationResponse
{
    public class DonationPackageResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }
        public int SubscribeLimit { get; set; }
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
