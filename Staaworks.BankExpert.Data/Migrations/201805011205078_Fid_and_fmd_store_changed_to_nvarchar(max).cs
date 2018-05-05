namespace Staaworks.BankExpert.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fid_and_fmd_store_changed_to_nvarcharmax : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Fingerprints", "Fmd", c => c.String());
            AlterColumn("dbo.Fingerprints", "Fid1", c => c.String());
            AlterColumn("dbo.Fingerprints", "Fid2", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Fingerprints", "Fid2", c => c.String(maxLength: 1023));
            AlterColumn("dbo.Fingerprints", "Fid1", c => c.String(maxLength: 1023));
            AlterColumn("dbo.Fingerprints", "Fmd", c => c.String(maxLength: 1023));
        }
    }
}
