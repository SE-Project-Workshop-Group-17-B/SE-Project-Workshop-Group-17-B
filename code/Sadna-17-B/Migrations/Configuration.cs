namespace Sadna_17_B.Migrations
{
    using Sadna_17_B.DataAccessLayer;
    using Sadna_17_B.DomainLayer.StoreDom;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.SQLite.EF6.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true; // false;
            AutomaticMigrationDataLossAllowed = true; // Be careful with this in production
            //SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator()); // the Golden Key
        }



        protected override void Seed(ApplicationDbContext context)
        {
            /*// Add initial data here
            if (!context.Stores.Any())
            {
                context.Stores.AddOrUpdate(
                    s => s.name,
                    new Store { name = "Store 1", Email = "store1@example.com", PhoneNumber = "123-456-7890", description = "First store", Address = "123 Main St" },
                    new Store { name = "Store 2", Email = "store2@example.com", PhoneNumber = "098-765-4321", description = "Second store", Address = "456 Elm St" }
                );

                context.SaveChanges();
            }*/

            /*if (!context.products.Any())
            {
                context.products.AddOrUpdate(
                    p => p.name,
                    new Product { name = "Product 1", price = 10.99, category = "category 1", storeId = 1, description = "First product", amount = 100 },
                    new Product { name = "Product 2", price = 20.99, category = "category 2", storeId = 1, description = "Second product", amount = 50 },
                    new Product { name = "Product 3", price = 15.99, category = "category 1", storeId = 2, description = "Third product", amount = 75 }
                );

                context.SaveChanges();
            }*/
        }
    }
}