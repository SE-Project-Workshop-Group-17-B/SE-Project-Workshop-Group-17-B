namespace Sadna_17_B.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class c : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StoreId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Description = c.String(),
                        Address = c.String(),
                        PurchasePolicySerialized = c.String(),
                        Rating = c.Double(nullable: false),
                        ReviewsSerialized = c.String(),
                        ComplaintsSerialized = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
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
                        Cart_ID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Carts", t => t.Cart_ID)
                .Index(t => t.Cart_ID);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserAge = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        storeId = c.Int(nullable: false),
                        amount = c.Int(nullable: false),
                        name = c.String(),
                        price = c.Double(nullable: false),
                        rating = c.Double(nullable: false),
                        category = c.String(),
                        description = c.String(),
                        ReviewsSerialized = c.String(),
                        ComplaintsSerialized = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Stores", t => t.storeId, cascadeDelete: true)
                .Index(t => t.storeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "storeId", "dbo.Stores");
            DropForeignKey("dbo.Orders", "Cart_ID", "dbo.Carts");
            DropForeignKey("dbo.Inventories", "StoreId", "dbo.Stores");
            DropIndex("dbo.Products", new[] { "storeId" });
            DropIndex("dbo.Orders", new[] { "Cart_ID" });
            DropIndex("dbo.Inventories", new[] { "StoreId" });
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
            DropTable("dbo.Orders");
            DropTable("dbo.Stores");
            DropTable("dbo.Inventories");
        }
    }
}
