namespace Sadna_17_B.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class c : DbMigration
    {
        public override void Up()
        {
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
                        DiscountPolicySerialized = c.String(),
                        PurchasePolicySerialized = c.String(),
                        Rating = c.Double(nullable: false),
                        ReviewsSerialized = c.String(),
                        ComplaintsSerialized = c.String(),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscribers", "ShoppingCart_ID", "dbo.Carts");
            DropForeignKey("dbo.Products", "storeId", "dbo.Stores");
            DropIndex("dbo.Products", new[] { "storeId" });
            DropIndex("dbo.Subscribers", new[] { "ShoppingCart_ID" });
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
            DropTable("dbo.Subscribers");
        }
    }
}
