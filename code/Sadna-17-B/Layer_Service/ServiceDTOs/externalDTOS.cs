using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.Layer_Service.ServiceDTOs
{
    public class handshakeDTO
    {
        public string action_type { get; set; }

        public handshakeDTO()
        {
            action_type = "handshake";
        }
    }

    public class payDTO
    {
        public string action_type { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string card_number { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string holder { get; set; }
        public string cvv { get; set; }
        public string ID { get; set; }

        public payDTO()
        {
            action_type = "pay";
        }
    }

    public class cancel_pay_DTO
    {
        public string action_type { get; set; }
        public string transaction_id { get; set; }

        public cancel_pay_DTO()
        {
            action_type = "cancel_pay";
        }
    }

    public class cancel_supply_DTO
    {
        public string ActionType { get; set; }
        public string TransactionId { get; set; }

        public cancel_supply_DTO()
        {
            ActionType = "cancel_supply";
        }
    }

    public class supplyDTO
    {
        public string action_type { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }

        public supplyDTO()
        {
            action_type = "supply";
        }
    }
    
}