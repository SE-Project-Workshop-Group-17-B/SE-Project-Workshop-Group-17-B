using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.Utils_external
{
    public class paymentDTO
    {
        public string action_type { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string card_number { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string holder { get; set; }
        public string cvv { get; set; }
        public string id { get; set; }

    }

    public class cancelDTO
    {
        public string action_type { get; set; }
        public string transaction_id { get; set; }
    }

    public class handshakeDTO
    {
        public string action_type { get; set; }
    }

    public class supplyDTO
    {
        public string action_type { get; set; }
        public string month { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }

    }

    public class handshakeDTO
    {
        public string action_type { get; set; }

    }
}