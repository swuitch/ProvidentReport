using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvidentUtility.Models
{
    public class Area
    {
        public int area_code { get; set; }
        public int delivery_days { get; set; }
        public int grace_period { get; set; }
        public double penalty_rate { get; set; }
        public double er_price { get; set; }
        public double ip_price { get; set; }
    }
}