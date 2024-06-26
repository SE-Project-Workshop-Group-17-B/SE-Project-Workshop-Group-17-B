using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.Generic;
using Sadna_17_B.DomainLayer.Entities;

namespace Sadna_17_B.Data
{
    public class TradingSystemContext : DbContext
    {
        public TradingSystemContext() : base("name=TradingSystemDB")
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Additional configurations can be added here if needed
        }
    }
}