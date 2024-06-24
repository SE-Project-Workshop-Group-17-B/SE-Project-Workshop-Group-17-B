

using Sadna_17_B.DomainLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Sadna_17_B.DomainLayer.StoreDom
{

    // ---------------- base purchase ----------------------------------------------------------------------------------------


    public abstract class Purchase
    {

        public static int ID;
        protected double price;
        public abstract bool apply_purchase(Cart cart);

        public Purchase()
        {
            ID++;
        }
    }

    // ---------------- immediate purchase ----------------------------------------------------------------------------------------


    public class Purchase_Immediate : Purchase
    {
        public override bool apply_purchase(Cart cart)
        {
            //implement purchase logic
            return true;
        }
    }

    

    // ---------------- bid / auction purchase ----------------------------------------------------------------------------------------


    public class BidPurchase : Purchase 
    {
        public double bid { get; set; }
        

        public void submit(double bid)
        {
            //implement bid logic
        }

        public void accept(double bid)
        {
            //implement bid accept logic
        }

        public void reject(double bid)
        {
            //implement bid accept logic
        }

        public void counter(double bid)
        {
            //implement bid accept logic
        }


        public override bool apply_purchase(Cart cart)
        {
            //implement purchase logic
            return true;
        }

    }


    public class Purchase_Auction : Purchase
    {
        public double StartingPrice { get; set; }
        public double HighestBid { get; set; }
        public string HighestBidder { get; set; }

        public override bool apply_purchase(Cart cart)
        {
            //implement purchase logic
            return true;
        }
    
    }


    // ---------------- luttery purchase ----------------------------------------------------------------------------------------


    public class Purchase_Luttery : Purchase
    {

        public int winner { get; set; }
        public double total { get; set; }
        public Dictionary<string, double> participants { get; set; }
       

        public void enter_luttery(string userName, double amount)
        {
            //implement luttery logic
        }

        public override bool apply_purchase(Cart cart)
        {
            //implement purchase logic
            return false;
        }

    }

}