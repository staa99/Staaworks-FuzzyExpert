using Staaworks.BankExpert.Data.Models;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Staaworks.BankExpert.Core.Repositories
{
    public static class UserRepository
    {
        private static Entities Entities { get; set; }
        
        private static void CheckToInit()
        {
            if (Entities == null || Entities.IsDisposed)
            {
                Entities = new Entities();
            }
        }


        public static User CreateUser(User user)
        {
            var dbUser = UserEntity.CreateFromUser(user);
            Entities.Users.Add(dbUser);
            Entities.SaveChanges();
            return dbUser;
        }


        public static bool SaveFingerPrint(Dictionary<int, string> Fmds, Dictionary<int, List<string>> Fids, string email, int? snapshotid)
        {
            if (snapshotid == null)
            {
                Snapshot snapshot = new Snapshot
                {
                    Details = ""
                };

                int userId = Entities.Users.Select(u => new { u.Id, u.Email }).Single(u => u.Email == email).Id;
                snapshot.OwnerId = userId;
                snapshot.CreatedById = userId;
                snapshot.DateCreated = DateTime.Now;
                Entities.Snapshots.Add(snapshot);

                foreach (int finger in Fmds.Keys)
                {
                    Fingerprint fingerprint = new Fingerprint
                    {
                        SnapshotId = snapshot.Id,
                        Fmd = Fmds[finger]
                    };
                    List<string> fids = Fids[finger];

                    fingerprint.Fid1 = fids[0];
                    fingerprint.Fid2 = fids[1];
                    Entities.Fingerprints.Add(fingerprint);
                }
                Entities.SaveChanges();
            }
            return true;
        }
    }
}
