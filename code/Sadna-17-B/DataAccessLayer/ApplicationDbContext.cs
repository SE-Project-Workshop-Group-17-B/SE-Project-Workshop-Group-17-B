using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public static string ApplicationDbContextName = "RemoteSadna-17-B-DB";
        public static string SQLiteDbContextName = "SQLiteDB";
        public static bool isMemoryDB = false;
        public ApplicationDbContext() : base("RemoteSadna-17-B-DB")
        {
        }

        public ApplicationDbContext(string connectionName) : base(connectionName)
        {
        }


        public DbSet<Store> Stores { get; set; }
        //public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SubOrder> SubOrders { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        //   public DbSet<Subscriber> Subscribers { get; set; }
        //  public DbSet<Guest> Guests { get; set; }
        //   public DbSet<Cart> Carts { get; set; }
        //   public DbSet<Cart_Product> CartProducts { get; set; }
        //     public DbSet<DiscountPolicy> DiscountPolicies { get; set; }
        //     public DbSet<PurchasePolicy> PurchasePolicies { get; set; }
        // public DbSet<Notification> Notifications { get; set; }
        // public DbSet<Offer> Offers { get; set; }

        // Add DbSet properties for other entities


        public string GetTableName(Type type)
        {
            var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Store Configuration
            modelBuilder.Entity<Store>()
                .HasKey(s => s.ID); // Primary Key - Disable Auto Increment (sort of?)


            // Inventory Configuration
            //modelBuilder.Entity<Inventory>()
            //    .HasKey(i => i.StoreId);
            //modelBuilder.Entity<Inventory>()
            //    .HasRequired(i => i.Store).WithRequiredDependent(s => s.Inventory);



            base.OnModelCreating(modelBuilder);
        }

        public void DeleteAll()
        {
            Stores.RemoveRange(Stores.ToList());
            SubOrders.RemoveRange(SubOrders.ToList());
            Orders.RemoveRange(Orders.ToList());
            Products.RemoveRange(Products.ToList());
            Subscribers.RemoveRange(Subscribers.ToList());
            Admins.RemoveRange(Admins.ToList());
            SaveChanges();
        }
    }
}