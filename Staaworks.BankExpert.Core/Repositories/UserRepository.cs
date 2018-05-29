using Staaworks.BankExpert.Data.Models;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Staaworks.BankExpert.Core.Repositories
{
    public static class UserRepository
    {
        private static Entities Entities { get; set; }

        private static void CheckToInit ()
        {
            if (Entities == null || Entities.IsDisposed)
            {
                Entities = new Entities();
            }
        }


        public static User CreateUser (User user)
        {
            CheckToInit();
            var dbUser = UserEntity.CreateFromUser(user);
            Entities.Users.Add(dbUser);
            Entities.SaveChanges();
            return dbUser;
        }


        public static User GetUser (string email)
        {
            CheckToInit();
            return Entities.Users.Where(u => u.Email == email).Include(u => u.Snapshots).Include(u => u.Snapshots.Select(us => us.Fingerprints)).SingleOrDefault();
        }


        public static bool SaveFingerPrint (Dictionary<int, string> Fmds, Dictionary<int, List<string>> Fids, string email, int? snapshotid)
        {
            CheckToInit();
            if (snapshotid == null)
            {
                var snapshot = new Snapshot
                {
                    Details = ""
                };

                var userId = Entities.Users.Select(u => new { u.Id, u.Email }).Single(u => u.Email == email).Id;
                snapshot.OwnerId = userId;
                snapshot.CreatedById = userId;
                snapshot.DateCreated = DateTime.Now;
                Entities.Snapshots.Add(snapshot);

                foreach (var finger in Fmds.Keys)
                {
                    var fingerprint = new Fingerprint
                    {
                        SnapshotId = snapshot.Id,
                        Fmd = Fmds[finger]
                    };
                    var fids = Fids[finger];

                    fingerprint.Fid1 = fids[0];
                    fingerprint.Fid2 = fids[1];
                    Entities.Fingerprints.Add(fingerprint);
                }
                Entities.SaveChanges();
            }
            return true;
        }

        public static bool LogSuccessfulEntry(string entry, string email)
        {
            CheckToInit();

            if (Entities.Logs.Any(l => l.CapturedData == entry))
            {
                return false;
            }

            Entities.Logs.Add(new FingerprintLogs
            {
                CapturedData = entry,
                Email = email
            });

            Entities.SaveChanges();

            return true;
        }
    }
}
