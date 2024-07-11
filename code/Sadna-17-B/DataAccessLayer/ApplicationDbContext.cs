using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
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
        public DbSet<Product> products { get; set; }

         public DbSet<Order> Orders { get; set; } // Add this line
      //  public DbSet<Discount> Discounts { get; set; }
      //  public DbSet<DiscountPolicy> DiscountPolicies { get; set; }
      //  public DbSet<PurchasePolicy> PurchasePolicies { get; set; }
      //  public DbSet<Purchase_Rule> PurchaseRules { get; set; }

        // Add DbSet properties for other entities
    }
}