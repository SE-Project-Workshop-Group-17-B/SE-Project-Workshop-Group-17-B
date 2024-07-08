using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class DiscountDTO
    {
        public int DiscountID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Strategy { get; set; }
        public string DiscountType { get; set; }
        public string Relevant { get; set; }
        public string Conditions { get; set; }
    }

}