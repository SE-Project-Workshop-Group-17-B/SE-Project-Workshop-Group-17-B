using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AggregationRule { get; set; }
        public string Conditions { get; set; }
    }

}