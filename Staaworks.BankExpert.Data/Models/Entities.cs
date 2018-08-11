namespace Staaworks.BankExpert.Data.Models
{
    using Staaworks.BankExpert.Shared.Models;
    using System.Data.Entity;

    public class Entities : DbContext
    {
        // Your context has been configured to use a 'Entities' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Staaworks.BankExpert.Data.Models.Entities' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Entities' 
        // connection string in the application configuration file.
        public Entities()
            : base("name=Entities")
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Fingerprint> Fingerprints { get; set; }
        public DbSet<FingerprintLogs> Logs { get; set; }

        public bool IsDisposed { get; private set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var userModelBuilder = modelBuilder.Entity<UserEntity>();
            userModelBuilder.Property(u => u.Name).HasMaxLength(255)
                                                  .IsRequired();

            userModelBuilder.Property(u => u.Email).IsRequired()
                                                   .HasMaxLength(255);
            userModelBuilder.HasIndex(u => u.Email).IsUnique();
            userModelBuilder.Property(u => u.Address).HasMaxLength(255).IsOptional();
            userModelBuilder.Property(u => u.HashSalt).IsRequired();
            userModelBuilder.Property(u => u.ImageLocation).HasMaxLength(1023).IsOptional();
            userModelBuilder.Property(u => u.Phone).HasMaxLength(50).IsOptional();
            userModelBuilder.Property(u => u.ZipOrPostalAddress).HasMaxLength(50).IsOptional();
            userModelBuilder.HasMany(u => u.Snapshots).WithRequired(s => s.Owner);
            userModelBuilder.HasMany(u => u.SnapshotsCreated).WithRequired(s => s.CreatedBy).WillCascadeOnDelete(false);
            userModelBuilder.HasMany(u => u.SnapshotsEdited).WithOptional(s => s.LastModifiedBy).WillCascadeOnDelete(false);

            userModelBuilder
                .Map(m =>
                {
                    m.Properties(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email
                    });
                    m.ToTable("Users");
                })
                .Map(m =>
                {
                    m.Properties(u => new
                    {
                        u.Id,
                        u.Address,
                        u.ZipOrPostalAddress,
                        u.Phone,
                        u.ImageLocation
                    });
                    m.ToTable("Profiles");
                })
                .Map(m =>
                {
                    m.Properties(u => new
                    {
                        u.Id,
                        u.PasswordHash,
                        u.HashSalt
                    });
                    m.ToTable("BasicAuthData");
                });


            var snapshotModelBuilder = modelBuilder.Entity<Snapshot>();
            snapshotModelBuilder.ToTable("Snapshots")
                                .HasKey(s => s.Id)
                                .Property(s => s.Details).HasMaxLength(500).IsOptional();

            snapshotModelBuilder.Property(s => s.CreatedById).IsRequired();
            snapshotModelBuilder.Property(s => s.LastModifiedById).IsOptional();
            snapshotModelBuilder.Property(s => s.OwnerId).IsRequired();
            snapshotModelBuilder.Property(s => s.DateCreated).IsRequired();
            snapshotModelBuilder.Property(s => s.LastModified).IsOptional();
            snapshotModelBuilder.HasMany(s => s.Fingerprints).WithRequired(f => f.Snapshot);


            var fingerprintModelBuilder = modelBuilder.Entity<Fingerprint>();
            fingerprintModelBuilder.ToTable("Fingerprints")
                                   .HasKey(f => f.Id)
                                   .Property(f => f.SnapshotId).IsRequired();
            fingerprintModelBuilder.Property(f => f.Fid1).HasColumnType("nvarchar(max)");
            fingerprintModelBuilder.Property(f => f.Fid2).HasColumnType("nvarchar(max)");
            fingerprintModelBuilder.Property(f => f.Fmd).HasColumnType("nvarchar(max)");
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}