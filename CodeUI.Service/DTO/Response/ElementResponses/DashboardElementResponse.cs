﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ElementResponses
{
    public class DashboardElementResponse
    {
        public BaseResponsePagingViewModel<ElementResponse> elementList { get; set; }

        public string? percent { get; set; }
    }
}
