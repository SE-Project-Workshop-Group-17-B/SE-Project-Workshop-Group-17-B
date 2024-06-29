using System.Data.Entity;
using Sadna_17_B.DomainLayer.Entities;

namespace Sadna_17_B.Data
{
    public class TradingSystemContext : DbContext
    {
        public TradingSystemContext() : base("name=TradingSystemDB")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TradingSystemContext>());
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure relationships and any additional constraints here
            modelBuilder.Entity<Store>()
                .HasMany(s => s.Products)
                .WithRequired(p => p.Store)
                .HasForeignKey(p => p.StoreID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithRequired(o => o.User)
                .HasForeignKey(o => o.UserID);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithRequired(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderID);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithRequired(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductID);
        }
    }
}