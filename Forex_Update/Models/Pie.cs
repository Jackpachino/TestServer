using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forex_Update.Models
{
    public class Pie
    {
        public decimal Total { get; set; }

        public Dictionary<string, decimal> Data { get; set; }
    }
}