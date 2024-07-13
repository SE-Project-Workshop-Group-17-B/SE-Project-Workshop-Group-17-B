namespace Sadna_17_B.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "Cart_ID", "dbo.Carts");
            //DropForeignKey("dbo.Stores", "inventory_store_id", "dbo.Inventories");
            DropIndex("dbo.Orders", new[] { "Cart_ID" });
            //DropIndex("dbo.Stores", new[] { "inventory_store_id" });
            CreateTable(
                "dbo.Subscribers",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        PasswordHash = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        ShoppingCart_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Username)
                .ForeignKey("dbo.Carts", t => t.ShoppingCart_ID)
                .Index(t => t.ShoppingCart_ID);
            
            AddColumn("dbo.Carts", "UserAge", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "storeId", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "ReviewsSerialized", c => c.String());
            AddColumn("dbo.Products", "ComplaintsSerialized", c => c.String());
            AddColumn("dbo.Stores", "PhoneNumber", c => c.String());
            AddColumn("dbo.Stores", "ReviewsSerialized", c => c.String());
            AddColumn("dbo.Stores", "ComplaintsSerialized", c => c.String());
            AddColumn("dbo.Stores", "status", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "storeId");
            //AddForeignKey("dbo.Products", "storeId", "dbo.Stores", "ID", cascadeDelete: true);
            DropColumn("dbo.Carts", "user_age");
            DropColumn("dbo.Products", "store_ID");
            DropColumn("dbo.Stores", "phone_number");
            DropColumn("dbo.Stores", "discount_policy_policy_name");
            //DropColumn("dbo.Stores", "inventory_store_id");
            //DropTable("dbo.Inventories");
            DropTable("dbo.Orders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        IsGuestOrder = c.Boolean(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        DestinationAddress = c.String(),
                        CreditCardInfo = c.String(),
                        TotalPrice = c.Double(nullable: false),
                        Cart_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderID);
            
            //CreateTable(
            //    "dbo.Inventories",
            //    c => new
            //        {
            //            store_id = c.Int(nullable: false, identity: true),
            //        })
            //    .PrimaryKey(t => t.store_id);
            
            //AddColumn("dbo.Stores", "inventory_store_id", c => c.Int());
            AddColumn("dbo.Stores", "discount_policy_policy_name", c => c.String());
            AddColumn("dbo.Stores", "phone_number", c => c.String());
            AddColumn("dbo.Products", "store_ID", c => c.Int(nullable: false));
            AddColumn("dbo.Carts", "user_age", c => c.Int(nullable: false));
            DropForeignKey("dbo.Subscribers", "ShoppingCart_ID", "dbo.Carts");
            //DropForeignKey("dbo.Products", "storeId", "dbo.Stores");
            DropIndex("dbo.Products", new[] { "storeId" });
            DropIndex("dbo.Subscribers", new[] { "ShoppingCart_ID" });
            DropColumn("dbo.Stores", "status");
            DropColumn("dbo.Stores", "ComplaintsSerialized");
            DropColumn("dbo.Stores", "ReviewsSerialized");
            DropColumn("dbo.Stores", "PhoneNumber");
            DropColumn("dbo.Products", "ComplaintsSerialized");
            DropColumn("dbo.Products", "ReviewsSerialized");
            DropColumn("dbo.Products", "storeId");
            DropColumn("dbo.Carts", "UserAge");
            DropTable("dbo.Subscribers");
            //CreateIndex("dbo.Stores", "inventory_store_id");
            CreateIndex("dbo.Orders", "Cart_ID");
            //AddForeignKey("dbo.Stores", "inventory_store_id", "dbo.Inventories", "store_id");
            AddForeignKey("dbo.Orders", "Cart_ID", "dbo.Carts", "ID", cascadeDelete: true);
        }
    }
}
