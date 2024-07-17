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
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using static Sadna_17_B.DomainLayer.User.NotificationSystem;
using static Sadna_17_B.DomainLayer.User.OfferSystem;

namespace Sadna_17_B.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public static string ApplicationDbContextName = "RemoteSadna-17-B-DB";
        public static string SQLiteDbContextName = "SQLiteDB";
        public static bool isMemoryDB = true; // Controls whether the actual database is connected or not
        public ApplicationDbContext() : base("RemoteSadna-17-B-DB")
        {
        }

        public ApplicationDbContext(string connectionName) : base(connectionName)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SubOrder> SubOrders { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<OwnerAppointmentOffer> OwnerAppointmentOffers { get; set; }
        public DbSet<ManagerAppointmentOffer> ManagerAppointmentOffers { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }
        public DbSet<Cart> Carts { get; set; }

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

        /* protected override void OnModelCreating(DbModelBuilder modelBuilder)
         {
             // Store Configuration
             modelBuilder.Entity<Store>()
                 .HasKey(s => s.StoreID); // Primary Key - Disable Auto Increment (sort of?)





             // Inventory Configuration
             //modelBuilder.Entity<Inventory>()
             //    .HasKey(i => i.StoreID);
             //modelBuilder.Entity<Inventory>()
             //    .HasRequired(i => i.Store).WithRequiredDependent(s => s.Inventory);



             base.OnModelCreating(modelBuilder);
         }*/


        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            // ... other configurations ...
            //
            //Store Configuration
            modelBuilder.Entity<Store>()
                .HasKey(s => s.StoreID); // Primary Key - Disable Auto Increment (sort of?)


            // OwnerAppointmentOffer Configuration - Composite Primary Key
            modelBuilder.Entity<OwnerAppointmentOffer>()
                .HasKey(o => new { o.AppointmentStoreID, o.SubscriberUsername });

            // ManagerAppointmentOffer Configuration - Composite Primary Key
            modelBuilder.Entity<ManagerAppointmentOffer>()
                .HasKey(o => new { o.AppointmentStoreID, o.SubscriberUsername });



            // OwnershipEntry configuration
            /*modelBuilder.Entity<OwnershipEntry>()
                .HasKey(oe => new { oe.SubscriberUsername, oe.StoreID });*/

            /* modelBuilder.Entity<OwnershipEntry>()
                 .HasRequired(oe => oe.Subscriber)
                 .WithMany(s => s.OwnershipEntries);*/
            // .HasForeignKey(oe => oe.SubscriberUsername);

            /*modelBuilder.Entity<OwnershipEntry>()
                .HasRequired(oe => oe.Owner)
                .WithMany()
                .HasForeignKey(oe => new { oe.OwnerID, oe.StoreID });*/

            // ManagementEntry configuration
            /*modelBuilder.Entity<ManagementEntry>()
                .HasKey(me => new { me.SubscriberUsername, me.StoreID });*/
            /*
                        modelBuilder.Entity<ManagementEntry>()
                            .HasRequired(me => me.Subscriber)
                            .WithMany(s => s.ManagementEntries);
                        //    .HasForeignKey(me => me.SubscriberUsername);

                        modelBuilder.Entity<ManagementEntry>()
                            .HasRequired(me => me.Manager)
                            .WithMany()
                            .HasForeignKey(me => new { me.ManagerID, me.StoreID });*/

            // ... other configurations ...

            base.OnModelCreating(modelBuilder);
        }


        public void DeleteAll()
        {
            SubOrders.RemoveRange(SubOrders.ToList());
            Orders.RemoveRange(Orders.ToList());
            Products.RemoveRange(Products.ToList());
            UserNotifications.RemoveRange(UserNotifications.ToList());
            OwnerAppointmentOffers.RemoveRange(OwnerAppointmentOffers.ToList());
            ManagerAppointmentOffers.RemoveRange(ManagerAppointmentOffers.ToList());
            Owners.RemoveRange(Owners.ToList());
            Managers.RemoveRange(Managers.ToList());
            Subscribers.RemoveRange(Subscribers.ToList());
            Admins.RemoveRange(Admins.ToList());
            Carts.RemoveRange(Carts.ToList());
            Stores.RemoveRange(Stores.ToList());
            SaveChanges();
        }
    }

    

}