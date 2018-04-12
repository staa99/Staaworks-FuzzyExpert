using IdentityBusinessLayer.PortalServ;
using IdentityCommons;
using IdentityCommons.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityBusinessLayer.Repositories
{
    public class PersonRepository
    {
        IdentityDataLayer.DataModels.IdentityDBEntities dbcontext;
       public PersonRepository()
        {
            dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
        }
        public List<string> getAllBG()
        {
           return dbcontext.BloodGroups.Select(p => p.BloodGroup1.Trim()).ToList();
        }


        public bool saveStudentPersonProfile( string personIdNo, StudentPerson newpersonprofile, int? snapshotid,bool overridewithnull=true)
        {

            //backup old data
            if (dbcontext.Persons.Any(p => p.IdentityNo == personIdNo))
            {
                IdentityDataLayer.DataModels.StudentPerson pid = dbcontext.StudentPersons.First(p => p.Person.IdentityNo == personIdNo);
                if (!dbcontext.IDSnapshots.Any(p => p.PersonId == pid.Person.Id && p.SnapshotDetails != ""))
                {
                    //back migrated data
                    IdentityDataLayer.DataModels.IDSnapshot idsnapshot = new IdentityDataLayer.DataModels.IDSnapshot();
                    idsnapshot.SnapshotDetails = serializeXML(pid);

                    idsnapshot.PersonId = pid.PersonId;
                    idsnapshot.CreatedBy = 1;
                    idsnapshot.DateCreated = DateTime.Now.AddDays(-1);
                    dbcontext.IDSnapshots.Add(idsnapshot);                 

                    dbcontext.SaveChanges();
                }
            }
            if (snapshotid == null)
            {
                IdentityDataLayer.DataModels.IDSnapshot idsnapshot = new IdentityDataLayer.DataModels.IDSnapshot();
                idsnapshot.SnapshotDetails = serializeXML(newpersonprofile);
                IdentityDataLayer.DataModels.Person person = dbcontext.Persons.First(p => p.IdentityNo == personIdNo);
                idsnapshot.PersonId = person.Id;
                idsnapshot.CreatedBy = 1;
                idsnapshot.DateCreated = DateTime.Now;
                dbcontext.IDSnapshots.Add(idsnapshot);

                person.Surname = newpersonprofile.Surname;
                person.Firstname = newpersonprofile.Firstname;
                person.Middlename = newpersonprofile.Middlename;
                person.HealthCentreNo = newpersonprofile.HCN??person.HealthCentreNo;
                person.BloodGroup = newpersonprofile.BG ?? person.BloodGroup;
                person.StudentPerson.CurrentLevel = String.IsNullOrEmpty(newpersonprofile.Level)? person.StudentPerson.CurrentLevel: newpersonprofile.Level;
                person.StudentPerson.StudentCategoryId = newpersonprofile.CategoryId;
                // String.IsNullOrEmpty(newpersonprofile.Level) ? person.StudentPerson.CurrentLevel : newpersonprofile.Level;
                                                                                     /* if (newpersonprofile.CategoryId == 2)
                                                                                      {
                                                                                          person.StudentPerson.Program =  newpersonprofile.Level;

                                                                                      }*/
                person.StudentPerson.Program = newpersonprofile.Programme;
                person.Phone = newpersonprofile.Phone??person.Phone;
                person.Email = newpersonprofile.Email??person.Email;
                person.Sex = newpersonprofile.Sex.ToString();
                person.StudentPerson.SponsorName = newpersonprofile.SponsorName?? person.StudentPerson.SponsorName;
                person.StudentPerson.SponsorAddress = newpersonprofile.SponsorAddress ?? person.StudentPerson.SponsorAddress;
                person.StudentPerson.SponsorPhone = newpersonprofile.SponsorPhone ?? person.StudentPerson.SponsorPhone;
                person.StudentPerson.CurrentDepartmentId = newpersonprofile.Department.Code==null? person.StudentPerson.CurrentDepartmentId:dbcontext.Departments.First(p => p.DepartmentCode == newpersonprofile.Department.Code).DepartmentId;
                person.StudentPerson.LastEnrollmentSession = String.IsNullOrEmpty(newpersonprofile.CurrentSession)? person.StudentPerson.LastEnrollmentSession : newpersonprofile.CurrentSession;
                person.StudentPerson.LastEnrollmentStatus = String.IsNullOrEmpty(newpersonprofile.CurrentStatus)? person.StudentPerson.LastEnrollmentStatus:newpersonprofile.CurrentStatus;

                dbcontext.SaveChanges();

            }
            dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
            return true;
        }
       
        public String serializeXML(StudentPerson sp)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>";
            xml += "<StudentPerson>";
           
            xml += "<PersonId>"+sp.PersonID+ "</PersonId>";
            xml += "<Surname>" + sp.Surname + "</Surname>";
            xml += "<Firstname>" + sp.Firstname + "</Firstname>";
            xml += "<Middlename>" + sp.Middlename + "</Middlename>";
            xml += "<BG>" + sp.BG + "</BG>";
            xml += "<CollegeCode>" + sp.Department.CollegeCode+ "<CollegeCode>";
            xml += "<Department>" + sp.Department.Code+ "</Department>";
            xml += "<Programme>" + sp.Programme + "</Programme>";
            xml += "<HCN>" + sp.HCN + "</HCN>";
            xml += "<Phone>" + sp.Phone + "</Phone>";
            xml += "<Email>" + sp.Email + "</Email>";
            xml += "<Level>" + sp.Level + "</Level>";
            xml += "<SponsorName>" + sp.SponsorName + "</SponsorName>";
            xml += "<SponsorAddress>" + sp.SponsorAddress + "</SponsorAddress>";
            xml += "<Sex>" + sp.Sex + "</Sex>";
           
            xml += "</StudentPerson>";
            return xml;


        }

        public String serializeXML(IdentityDataLayer.DataModels.StudentPerson sp)
        {
           
            string xml = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>";
            xml += "<StudentPerson>";

            xml += "<PersonId>" + sp.Person.IdentityNo + "</PersonId>";
            xml += "<Surname>" + sp.Person.Surname + "</Surname>";
            xml += "<Firstname>" + sp.Person.Firstname + "</Firstname>";
            xml += "<Middlename>" + sp.Person.Middlename + "</Middlename>";
            xml += "<BG>" + sp.Person.BloodGroup + "</BG>";
            xml += "<CollegeCode>" + sp.Department.CollegeCode + "<CollegeCode>";
            xml += "<Department>" + sp.Department.DepartmentName + "</Department>";
            xml += "<Programme>" + sp.Program + "</Programme>";
            xml += "<HCN>" + sp.Person.HealthCentreNo + "</HCN>";
            xml += "<Phone>" + sp.Person.Phone + "</Phone>";
            xml += "<Email>" + sp.Person.Email + "</Email>";
            xml += "<Level>" + sp.CurrentLevel + "</Level>";
            xml += "<SponsorName>" + sp.SponsorName + "</SponsorName>";
            xml += "<SponsorAddress>" + sp.SponsorAddress + "</SponsorAddress>";
            xml += "<Sex>" + sp.Person.Sex + "</Sex>";

            xml += "</StudentPerson>";
            return xml;


        }
        public StudentPerson deSerializeXML(string  xml)
        {
            StudentPerson sp = null;// new StudentPerson()
           /* string xml = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>";
            xml += "<StudentPerson>";

            xml += "<PersonId>" + sp.PersonID + "</PersonId>";
            xml += "<Surname>" + sp.Surname + "</Surname>";
            xml += "<Firstname>" + sp.Firstname + "</Firstname>";
            xml += "<Middlename>" + sp.Middlename + "</Middlename>";
            xml += "<BG>" + sp.BG + "</BG>";
            xml += "<CollegeCode>" + sp.Department.CollegeCode + "<CollegeCode>";
            xml += "<Department>" + sp.Department.Name + "</Department>";
            xml += "<HCN>" + sp.HCN + "</HCN>";
            xml += "<Phone>" + sp.Phone + "</Phone>";
            xml += "<Email>" + sp.Email + "</Email>";
            xml += "<Level>" + sp.Level + "</Level>";
            xml += "<SponsorName>" + sp.SponsorName + "</SponsorName>";
            xml += "<SponsorAddress>" + sp.SponsorAddress + "</SponsorAddress>";
            xml += "<Sex>" + sp.Sex + "</Sex>";

            xml += "</StudentPerson>";*/
            return sp;


        }
        public bool savePersonPic(byte[] photo, string personIdNo,int? snapshotid)
        {

            //backup old data
            if (dbcontext.Persons.Any(p => p.IdentityNo == personIdNo && p.CurrentPhoto != null ))
            {
                IdentityDataLayer.DataModels.Person pid = dbcontext.Persons.First(p => p.IdentityNo == personIdNo && p.CurrentPhoto != null);
                if (!dbcontext.PersonPictures.Any(p => p.IDSnapshot.PersonId == pid.Id && p.OriginalImage != null))
                {
                    //back migrated data
                    IdentityDataLayer.DataModels.IDSnapshot idsnapshot = new IdentityDataLayer.DataModels.IDSnapshot();
                    idsnapshot.SnapshotDetails = "";
                    
                    idsnapshot.PersonId = pid.Id;
                    idsnapshot.CreatedBy = 1;
                    idsnapshot.DateCreated = DateTime.Now.AddDays(-1);
                    dbcontext.IDSnapshots.Add(idsnapshot);
                    IdentityDataLayer.DataModels.PersonPicture pc = new IdentityDataLayer.DataModels.PersonPicture();
                    pc.IDSnapshot = idsnapshot;
                    pc.ProcessedImage = pid.WatermarkedPhoto;
                    pc.OriginalImage = pid.CurrentPhoto;
                    pc.CreatedBy = 1;
                    pc.DateCreated = DateTime.Now.AddDays(-1); ;
                    dbcontext.PersonPictures.Add(pc);
                   

                    dbcontext.SaveChanges();
                }
            }
            if (snapshotid == null)
            {
                IdentityDataLayer.DataModels.IDSnapshot idsnapshot = new IdentityDataLayer.DataModels.IDSnapshot();
                idsnapshot.SnapshotDetails = "";
                IdentityDataLayer.DataModels.Person person = dbcontext.Persons.First(p=>p.IdentityNo== personIdNo);
                idsnapshot.PersonId = person.Id;
                idsnapshot.CreatedBy = 1;
                idsnapshot.DateCreated = DateTime.Now;
                dbcontext.IDSnapshots.Add(idsnapshot);
                IdentityDataLayer.DataModels.PersonPicture pc = new IdentityDataLayer.DataModels.PersonPicture();
                pc.IDSnapshot = idsnapshot;
                pc.ProcessedImage = photo;
                pc.OriginalImage = photo;
                pc.CreatedBy = 1;
                pc.DateCreated = DateTime.Now;
                dbcontext.PersonPictures.Add(pc);
                person.CurrentPhoto = photo;
                //watermaked picture;
                MemoryStream ms = new MemoryStream(photo, 0, photo.Length, true, true);

               byte [] bytes = Util.AddWaterMark(ms, "FUNAAB");
                person.WatermarkedPhoto = bytes;
                dbcontext.SaveChanges();
                
            }
            dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
            return true;
        }



        public Dictionary<int, string>  getPersonFmds(string personIdNo, bool distinctfingers = true)
        {
            Dictionary<int, string> Fmds = null;
            if (dbcontext.Persons.Any(p => p.IdentityNo == personIdNo ))
            {
                IdentityDataLayer.DataModels.Person pid = dbcontext.Persons.First(p => p.IdentityNo == personIdNo);
                if (dbcontext.PersonFingers.Any(p => p.IDSnapshot.PersonId == pid.Id && p.FMD != null))
                {
                    Fmds = new Dictionary<int, string>();
                    List<IdentityDataLayer.DataModels.PersonFinger> pfs = dbcontext.PersonFingers.Where(p => p.IDSnapshot.PersonId == pid.Id && p.FMD != null).OrderByDescending(p=>p.SnapshotId).ToList();
                    foreach(IdentityDataLayer.DataModels.PersonFinger pf in pfs)
                    {
                        if (!Fmds.Any(p => p.Key == pf.FingerId))
                        {
                            Fmds.Add(pf.FingerId, pf.FMD);
                        }
                    }
                }
            }
            return Fmds;
        }
        public  bool saveFingerPrint(Dictionary<int, string> Fmds, Dictionary<int, List<string>> Fids, string personIdNo, int? snapshotid)
        {
            if (snapshotid == null)
            {
                IdentityDataLayer.DataModels.IDSnapshot idsnapshot = new IdentityDataLayer.DataModels.IDSnapshot();
                idsnapshot.SnapshotDetails = "";
                IdentityDataLayer.DataModels.Person person = dbcontext.Persons.First(p => p.IdentityNo == personIdNo);
                idsnapshot.PersonId = person.Id;
                idsnapshot.CreatedBy = 1;
                idsnapshot.DateCreated = DateTime.Now;
                dbcontext.IDSnapshots.Add(idsnapshot);
                foreach(int finger in Fmds.Keys)
                {

               
                IdentityDataLayer.DataModels.PersonFinger pf = new IdentityDataLayer.DataModels.PersonFinger();
                pf.IDSnapshot = idsnapshot;
                pf.FingerId = finger;
                pf.FMD = Fmds[finger];
                pf.CreatedBy = 1;
                    List<string> fds = Fids[finger];

                    pf.FID1 = fds[0];
                    pf.FID2 = fds[1];
                    pf.FID3 = fds[2];
                    pf.FID4 = fds[3];
                    pf.DateCreated = DateTime.Now;
                dbcontext.PersonFingers.Add(pf);
                }
                dbcontext.SaveChanges();
            }
            dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
            return true;
        }
        private bool updatefromsource(String id)
        {
            string cuurentsession = "";
            string currentsession= dbcontext.Configs.First(p => p.ConfigName == "CurrentSession").ConfigValue;
            StudentPerson sp = getStudentPersonFromSouce(id, currentsession);
            if (!dbcontext.StudentPersons.Any(p => p.Person.IdentityNo == id))
            {
                IdentityDataLayer.DataModels.Person ip = new IdentityDataLayer.DataModels.Person();
                ip.IdentityNo = sp.PersonID;
                ip.Surname = sp.Surname;
                ip.Firstname = sp.Firstname;
                ip.Middlename = sp.Middlename;
                ip.Phone = sp.Phone;
                ip.Email = sp.Email;
                ip.Address = sp.Address;
                ip.HealthCentreNo = sp.HCN;
                ip.Sex = sp.Sex.ToString();
                dbcontext.Persons.Add(ip);
               // dbcontext.SaveChanges();
               // dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
               // ip = dbcontext.Persons.Single(p => p.IdentityNo == sp.PersonID);
                IdentityDataLayer.DataModels.StudentPerson isp = new IdentityDataLayer.DataModels.StudentPerson();
                isp.CurrentLevel = sp.Level;
                isp.Program = sp.Programme;
                isp.SponsorName = sp.SponsorName;
                isp.SponsorPhone = sp.SponsorPhone;
                isp.SponsorAddress = sp.SponsorAddress;
                isp.StudentCategoryId = sp.CategoryId;
                isp.CurrentDepartmentId = dbcontext.Departments.First(p => p.DepartmentCode == sp.Department.Code).DepartmentId;
                isp.CreatedBy = 1;
                isp.DateCreated = DateTime.Now;
                isp.Person = ip;
                isp.LastEnrollmentSession = sp.CurrentSession;
                isp.LastEnrollmentStatus = sp.CurrentStatus;
                dbcontext.StudentPersons.Add(isp);
                dbcontext.SaveChanges();
            }
            else
            {
               IdentityDataLayer.DataModels.StudentPerson isp= dbcontext.StudentPersons.First(p => p.Person.IdentityNo == id);
                if (!match(sp, isp))
                {
                    bool b = saveStudentPersonProfile(id, sp, null);
                    return b;
                }
            }
            dbcontext = new IdentityDataLayer.DataModels.IdentityDBEntities();
            return true;
        }
        private bool match(StudentPerson sp, IdentityDataLayer.DataModels.StudentPerson isp)
        {
            string s1 = sp.PersonID +
             sp.Surname +
            sp.Firstname +
            sp.Middlename +
            sp.Department.CollegeCode +
            sp.Department.Code +
            sp.Phone +
            sp.Sex +
            sp.Email +
            sp.Level +
            sp.SponsorName +
            sp.SponsorPhone +
            sp.SponsorAddress +
            sp.CurrentStatus +
            sp.CurrentSession;

            string s2 = isp.Person.IdentityNo +
            isp.Person.Surname +
           isp.Person.Firstname +
           isp.Person.Middlename +
           isp.Department.CollegeCode +
           isp.Department.DepartmentCode +
           isp.Person.Phone +
           isp.Person.Sex +
           isp.Person.Email +
           isp.CurrentLevel +
           isp.SponsorName +
           isp.SponsorPhone +
           isp.SponsorAddress +
           isp.LastEnrollmentStatus +
           isp.LastEnrollmentSession;

            return s1==s2;
        }
        public StudentPerson getStudentPersonById(String id, bool updatefromsource = false)
        {
            IEnumerable<StudentPerson> results = new List<StudentPerson>();
            List<IdentityDataLayer.DataModels.StudentPerson> presults = new List<IdentityDataLayer.DataModels.StudentPerson>();

            if (updatefromsource)
            {

                this.updatefromsource(id);
            }
          //  if (!updatefromsource)
          //  {



                presults.AddRange(dbcontext.StudentPersons.Where(p => p.Person.IdentityNo == id));


           // }
            //results = from a in presults select new StaffPerson { PersonID = a.IdentityNo, Surname = a.Surname, Firstname = a.Firstname, Address = a.Address, Middlename = a.Middlename, BG = a.BloodGroup, HCN = a.HealthCentreNo, NextOfKin = new Person() };
            results = from a in presults select new StudentPerson(a,getPersonFmds(a.Person.IdentityNo));

            return results.Count() == 0 ? null : results.First();
        }

        public StudentRecord getStudentRecordFromSouce(string id,string session, out int categoryid) {
            ProfileServiceClient client = new ProfileServiceClient();
            categoryid = 1;
            StudentRecord sr= client.getStudentRecordWithSession(id, session);
            if (sr == null ||sr.MatricNo==null)
            {
                if (dbcontext.Configs.Any(p => p.ConfigName == "PGCurrentSession"))
                {
                    session = dbcontext.Configs.First(p => p.ConfigName == "PGCurrentSession").ConfigValue;
                }
                sr = getStudentRecordFromPGSouce(id, session);
                categoryid = 2;
            }
            return sr;
        }
        public StudentRecord getStudentRecordFromPGSouce(string id, string session)
        {
            PGService.PGServiceClient client = new PGService.PGServiceClient();
           PGService.StudentRecord sr_pg = client.getStudentRecordWithSession(id, session);
            StudentRecord sr = null;
            if (sr_pg != null)
            {
                sr = new StudentRecord();
                sr.MatricNo=sr_pg.MatricNo;
                sr.Surname=sr_pg.Surname;
                sr.Firstname=sr_pg.Firstname;

                sr.Middlename=sr_pg.Middlename;
                sr.Email = sr_pg.Email;
                sr.Phone = sr_pg.Phone;
                sr.Sex=sr_pg.Sex;
                sr.Department = sr_pg.Department;
               // sr.College=sr_pg.

              // sr.Level=sr_pg.le
               // sr.SponsorName;
               // sr.SponsorPhone;
               // sr.SponsorAddress=sr_pg.sp;
                sr.PaymentStatus=sr_pg.Status;
                sr.Level = sr_pg.ProgramCategory;
                sr.Programme = sr_pg.Programme;
            }
            return sr;
        }
        public StudentPerson getStudentPersonFromSouce(string id, string session)
        {
            // ProfileServiceClient client = new ProfileServiceClient();
            int categoryid = 0;
            StudentRecord sr = getStudentRecordFromSouce(id, session,out categoryid);
            StudentPerson sp = new StudentPerson();
            sp.PersonID =sr.MatricNo;
            sp.CategoryId = categoryid;
            sp.Surname = sr.Surname;
            sp.Firstname = sr.Firstname;
           // sp.Address = sr.Address;
            sp.Middlename = sr.Middlename;
            sp.Phone = sr.Phone;
            sp.Sex = sr.Sex[0];
           
            sp.NextOfKin = new Person();
            
            sp.Email = sr.Email;
            sp.Department = new Department();
            if (sr.Department != null)
            {
                sp.Department = new Department(sr.Department, sr.Department, sr.College);
            }
            sp.Level = sr.Level;
            sp.SponsorName = sr.SponsorName;
            sp.SponsorPhone = sr.SponsorPhone;
            sp.SponsorAddress = sr.SponsorAddress;
            sp.CurrentStatus = sr.PaymentStatus;
            sp.Programme = sr.Programme;
            sp.CurrentSession = session;
            return sp;
        }
        public StaffPerson getStaffPersonById(String id, bool updatefromsource = false)
        {
            IEnumerable<StaffPerson> results = new List<StaffPerson>();
            List<IdentityDataLayer.DataModels.StaffPerson> presults = new List<IdentityDataLayer.DataModels.StaffPerson>();
           // if (!updatefromsource)
           // {

            

                    presults.AddRange(dbcontext.StaffPersons.Where(p => p.Person.IdentityNo == id));

               
           // }
            //results = from a in presults select new StaffPerson { PersonID = a.IdentityNo, Surname = a.Surname, Firstname = a.Firstname, Address = a.Address, Middlename = a.Middlename, BG = a.BloodGroup, HCN = a.HealthCentreNo, NextOfKin = new Person() };
            results = from a in presults select new StaffPerson (a);

            return results.Count()==0? null :results.First();
        }
        public List<Person> getPersons(String id, String surname, String firstanme, persontype persontype = persontype.ALL, bool updatefromsource = false)
        {
            IEnumerable<Person> results = new List<Person>();
            List<IdentityDataLayer.DataModels.Person> presults = new List<IdentityDataLayer.DataModels.Person>();
           // if (!updatefromsource)
           // {

                if (persontype == persontype.ALL)
                {
                    
                    presults.AddRange(dbcontext.Persons.Where(p => (id=="" ||p.IdentityNo == id) && ( surname=="" ||p.Surname==surname ) && (firstanme == "" || p.Firstname == firstanme)));

                }
                else if (persontype == persontype.STUDENT)
                {

                    presults.AddRange(dbcontext.StudentPersons.Where(p => (id == "" || p.Person.IdentityNo == id) && (surname == "" || p.Person.Surname == surname) && (firstanme == "" || p.Person.Firstname == firstanme)).Select(p=>p.Person));

                }
                else if (persontype == persontype.STAFF)
                {

                    presults.AddRange(dbcontext.StaffPersons.Where(p => (id == "" || p.Person.IdentityNo == id) && (surname == "" || p.Person.Surname == surname) && (firstanme == "" || p.Person.Firstname == firstanme)).Select(p => p.Person));

                }
                else if (persontype == persontype.GENERAL)
                {

                    presults.AddRange(dbcontext.GenericPersons.Where(p => (id == "" || p.Person.IdentityNo == id) && (surname == "" || p.Person.Surname == surname) && (firstanme == "" || p.Person.Firstname == firstanme)).Select(p => p.Person));

                }

           // }
           // results = from a in presults select new Person { PersonID=a.IdentityNo,Surname=a.Surname,Firstname=a.Firstname, Address=a.Address, Middlename=a.Middlename, BG=a.BloodGroup, HCN= a.HealthCentreNo, NextOfKin= new Person()  };
            results = from a in presults select new Person (a);

            return results.ToList();
        }
    }
}
