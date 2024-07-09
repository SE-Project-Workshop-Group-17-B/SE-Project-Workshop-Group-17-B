using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class ProductDTO
    {
        public int ID { get; set; }
        public int StoreID { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Reviews { get; set; }
        public bool Locked { get; set; }

    }
}