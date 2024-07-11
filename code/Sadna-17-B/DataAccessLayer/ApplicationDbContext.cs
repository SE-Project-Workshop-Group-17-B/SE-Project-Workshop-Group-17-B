using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext() : base("RemoteSadna-17-B-DB")
        {
        }


        public DbSet<Store> Stores { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
     //   public DbSet<Subscriber> Subscribers { get; set; }
      //  public DbSet<Guest> Guests { get; set; }
     //   public DbSet<Cart> Carts { get; set; }
     //   public DbSet<Cart_Product> CartProducts { get; set; }
   //     public DbSet<DiscountPolicy> DiscountPolicies { get; set; }
   //     public DbSet<PurchasePolicy> PurchasePolicies { get; set; }
       // public DbSet<Notification> Notifications { get; set; }
       // public DbSet<Offer> Offers { get; set; }

        // Add DbSet properties for other entities



    }
}