using System;
using System.Collections.Generic;

namespace Staaworks.BankExpert.Shared.Models
{
    public class User
    {
        public User()
        {
            Snapshots = new HashSet<Snapshot>();
            SnapshotsCreated = new HashSet<Snapshot>();
            SnapshotsEdited = new HashSet<Snapshot>();
        }
        // General Profile Data
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string HashSalt { get; set; }
        public string ZipOrPostalAddress { get; set; }
        public string ImageLocation { get; set; }

        //Fingerprint
        public virtual ICollection<Snapshot> Snapshots { get; set; }
        public virtual ICollection<Snapshot> SnapshotsCreated { get; set; }
        public virtual ICollection<Snapshot> SnapshotsEdited { get; set; }
        public virtual ICollection<FingerprintLogs> FingerprintLogs { get; set; }
    }



    public class Snapshot
    {
        public Snapshot()
        {
            Fingerprints = new HashSet<Fingerprint>();
        }
        public int Id { get; set; }
        public string Details { get; set; }
        public int OwnerId { get; set; }
        public int CreatedById { get; set; }
        public int? LastModifiedById { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual UserEntity CreatedBy { get; set; }
        public virtual UserEntity LastModifiedBy { get; set; }
        public virtual UserEntity Owner { get; set; }
        public virtual ICollection<Fingerprint> Fingerprints { get; set; }
    }



    public class Fingerprint
    {
        public int Id { get; set; }
        public int SnapshotId { get; set; }
        public string Fmd { get; set; }
        public string Fid1 { get; set; }
        public string Fid2 { get; set; }

        public virtual Snapshot Snapshot { get; set; }
    }
}
