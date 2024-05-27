using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DomainLayer.Store
{


    // ---------------- Policy ----------------------------------------------------------------------------------------


    public class PurchasePolicy
    {

        // ---------------- variables ----------------------------------------------------


        public string PolicyName { get; set; }
        public List<PurchaseType> AllowedPurchaseTypes { get; set; }



        // ---------------- functions -----------------------------------------------------


        public void AllowPurchaseType(PurchaseType type)
        {
            AllowedPurchaseTypes.Add(type);
        }

        public void AddPurchaseType(PurchaseType type)
        {
            AllowedPurchaseTypes.Add(type);
        }

        public void RemovePurchaseType(PurchaseType type)
        {
            AllowedPurchaseTypes.Remove(type);
        }
    }


    // ---------------- Purchase ----------------------------------------------------------------------------------------


    public abstract class PurchaseType
    {
        protected double price;
        public abstract void executePurchase();
    }


    public class ImmediatePurchase : PurchaseType
    {
        public override void executePurchase()
        {
            //implement purchase logic

        }
    }

    public class BidPurchase : PurchaseType
    {
        public double BidPrice { get; set; }
        public override void executePurchase()
        {
            //implement purchase logic
        }

        public void submitBid(double bidPrice)
        {
            //implement bid logic
        }

        public void AcceptBid(double bidPrice)
        {
            //implement bid accept logic
        }

        public void RejectBid(double bidPrice)
        {
            //implement bid accept logic
        }

        public void CounterBid(double bidPrice)
        {
            //implement bid accept logic
        }
    }


    public class AuctionPurchase : PurchaseType
    {
        public double StartingPrice { get; set; }
        public double HighestBid { get; set; }
        public string HighestBidder { get; set; }

        public override void executePurchase()
        {
            //implement purchase logic
        }
    }

    public class LutteryPurchase : PurchaseType
    {
        public double TotalAmountCollected { get; set; }
        public Dictionary<string, double> Participants { get; set; }
        public int WinningTicket { get; set; }
        public override void executePurchase()
        {
            //implement purchase logic
        }

        public void EnterLuttery(string userName, double amount)
        {
            //implement luttery logic
        }

    }
}

    
