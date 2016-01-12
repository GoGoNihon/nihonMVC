namespace GoGoNihon_MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changekey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Schools", "locationID", "dbo.Locations");
            DropPrimaryKey("dbo.Locations");
            DropColumn("dbo.Locations", "locationID");
            AddColumn("dbo.Locations", "id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Locations", "id");
            AddForeignKey("dbo.Schools", "locationID", "dbo.Locations", "id");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "locationID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Schools", "locationID", "dbo.Locations");
            DropPrimaryKey("dbo.Locations");
            DropColumn("dbo.Locations", "id");
            AddPrimaryKey("dbo.Locations", "locationID");
            AddForeignKey("dbo.Schools", "locationID", "dbo.Locations", "locationID");
        }
    }
}
