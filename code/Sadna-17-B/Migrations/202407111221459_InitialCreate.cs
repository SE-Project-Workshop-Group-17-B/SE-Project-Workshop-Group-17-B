namespace Sadna_17_B.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        email = c.String(),
                        phone_number = c.String(),
                        description = c.String(),
                        address = c.String(),
                        inventory_store_id = c.Int(nullable: false),
                        discount_policy_policy_name = c.String(),
                        rating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stores");
        }
    }
}
