using Sadna_17_B.DomainLayer.StoreDom;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Sadna_17_B_API.Models
{
    public class ProductDTOAPI
    {

        public int ID { get; set; }
        public int storeId { get; set; }
        public int amount { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double rating { get; set; }
        public string category { get; set; }
        public string description { get; set; } // up to 100 char

        public virtual ICollection<String> Reviews { get; set; }

        public virtual ICollection<String> Complaints { get; set; }
        public Dictionary<string, string> doc { get; set; }

    }
}
