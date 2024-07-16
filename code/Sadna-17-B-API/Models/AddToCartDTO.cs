using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B_API.Models
{
    public class AddToCartDTO
    {
        public Dictionary<string, string> Doc { get; set; }
        public int Change { get; set; }
    }
}