namespace Staaworks.BankExpert.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class replayattackblock : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FingerprintLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        CapturedData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FingerprintLogs");
        }
    }
}
