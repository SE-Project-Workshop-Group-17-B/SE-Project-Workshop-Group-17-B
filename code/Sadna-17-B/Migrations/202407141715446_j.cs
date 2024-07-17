namespace Sadna_17_B.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class j : DbMigration
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
                "dbo.ManagementEntries",
                c => new
                    {
                        SubscriberUsername = c.String(nullable: false, maxLength: 128),
                        StoreID = c.Int(nullable: false),
                        ManagerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubscriberUsername, t.StoreID })
                .ForeignKey("dbo.Managers", t => t.ManagerID, cascadeDelete: true)
                .ForeignKey("dbo.Subscribers", t => t.SubscriberUsername, cascadeDelete: true)
                .Index(t => t.SubscriberUsername)
                .Index(t => t.ManagerID);
            
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        ManagerID = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        AuthorizationsString = c.String(),
                    })
                .PrimaryKey(t => t.ManagerID);
            
            CreateTable(
                "dbo.OwnershipEntries",
                c => new
                    {
                        SubscriberUsername = c.String(nullable: false, maxLength: 128),
                        StoreID = c.Int(nullable: false),
                        OwnerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubscriberUsername, t.StoreID })
                .ForeignKey("dbo.Owners", t => t.OwnerID, cascadeDelete: true)
                .ForeignKey("dbo.Subscribers", t => t.SubscriberUsername, cascadeDelete: true)
                .Index(t => t.SubscriberUsername)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        OwnerID = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        IsFounder = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OwnerID);
            
            CreateTable(
                "dbo.ManagerAppointmentEntries",
                c => new
                    {
                        AppointingManagerID = c.Int(nullable: false, identity: true),
                        AppointedManagerUsername = c.String(),
                        AppointedManager_ManagerID = c.Int(),
                        AppointingOwner_OwnerID = c.Int(),
                    })
                .PrimaryKey(t => t.AppointingManagerID)
                .ForeignKey("dbo.Managers", t => t.AppointedManager_ManagerID)
                .ForeignKey("dbo.Owners", t => t.AppointingOwner_OwnerID)
                .Index(t => t.AppointedManager_ManagerID)
                .Index(t => t.AppointingOwner_OwnerID);
            
            CreateTable(
                "dbo.OwnerAppointmentEntries",
                c => new
                    {
                        AppointingOwnerID = c.Int(nullable: false, identity: true),
                        AppointedOwnerUsername = c.String(),
                        AppointedOwner_OwnerID = c.Int(),
                        AppointingOwner_OwnerID = c.Int(),
                        Owner_OwnerID = c.Int(),
                    })
                .PrimaryKey(t => t.AppointingOwnerID)
                .ForeignKey("dbo.Owners", t => t.AppointedOwner_OwnerID)
                .ForeignKey("dbo.Owners", t => t.AppointingOwner_OwnerID)
                .ForeignKey("dbo.Owners", t => t.Owner_OwnerID)
                .Index(t => t.AppointedOwner_OwnerID)
                .Index(t => t.AppointingOwner_OwnerID)
                .Index(t => t.Owner_OwnerID);
            
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
                        StoreID = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.StoreID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "storeId", "dbo.Stores");
            DropForeignKey("dbo.Subscribers", "ShoppingCart_ID", "dbo.Carts");
            DropForeignKey("dbo.OwnershipEntries", "SubscriberUsername", "dbo.Subscribers");
            DropForeignKey("dbo.OwnershipEntries", "OwnerID", "dbo.Owners");
            DropForeignKey("dbo.OwnerAppointmentEntries", "Owner_OwnerID", "dbo.Owners");
            DropForeignKey("dbo.OwnerAppointmentEntries", "AppointingOwner_OwnerID", "dbo.Owners");
            DropForeignKey("dbo.OwnerAppointmentEntries", "AppointedOwner_OwnerID", "dbo.Owners");
            DropForeignKey("dbo.ManagerAppointmentEntries", "AppointingOwner_OwnerID", "dbo.Owners");
            DropForeignKey("dbo.ManagerAppointmentEntries", "AppointedManager_ManagerID", "dbo.Managers");
            DropForeignKey("dbo.ManagementEntries", "SubscriberUsername", "dbo.Subscribers");
            DropForeignKey("dbo.ManagementEntries", "ManagerID", "dbo.Managers");
            DropIndex("dbo.Products", new[] { "storeId" });
            DropIndex("dbo.OwnerAppointmentEntries", new[] { "Owner_OwnerID" });
            DropIndex("dbo.OwnerAppointmentEntries", new[] { "AppointingOwner_OwnerID" });
            DropIndex("dbo.OwnerAppointmentEntries", new[] { "AppointedOwner_OwnerID" });
            DropIndex("dbo.ManagerAppointmentEntries", new[] { "AppointingOwner_OwnerID" });
            DropIndex("dbo.ManagerAppointmentEntries", new[] { "AppointedManager_ManagerID" });
            DropIndex("dbo.OwnershipEntries", new[] { "OwnerID" });
            DropIndex("dbo.OwnershipEntries", new[] { "SubscriberUsername" });
            DropIndex("dbo.ManagementEntries", new[] { "ManagerID" });
            DropIndex("dbo.ManagementEntries", new[] { "SubscriberUsername" });
            DropIndex("dbo.Subscribers", new[] { "ShoppingCart_ID" });
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
            DropTable("dbo.OwnerAppointmentEntries");
            DropTable("dbo.ManagerAppointmentEntries");
            DropTable("dbo.Owners");
            DropTable("dbo.OwnershipEntries");
            DropTable("dbo.Managers");
            DropTable("dbo.ManagementEntries");
            DropTable("dbo.Subscribers");
        }
    }
}
