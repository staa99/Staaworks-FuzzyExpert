using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCommons.Models
{
    public class StudentPerson : Person
    {
       
        public Department Department {get;set;}
        public string Level { get; set; }
        public string Category { get; set; }
        public string SponsorName { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorAddress { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentSession { get; set; }
        public string Programme { get; set; }
        public int CategoryId { get; set; }
        public StudentPerson()
        {



        }
        public StudentPerson(IdentityDataLayer.DataModels.Person a):base(a)
        {



        }
        public StudentPerson(IdentityDataLayer.DataModels.Person a, Dictionary<int, string> Fmds) : base(a, Fmds)
        {



        }
        public StudentPerson(IdentityDataLayer.DataModels.StudentPerson a):base(a.Person)
        {

            Department = new Department();
            if (a.Department != null)
            {
                Department = new Department(a.Department.DepartmentCode, a.Department.DepartmentName,a.Department.CollegeCode);
            }
            Level = a.CurrentLevel;
            SponsorName = a.SponsorName;
            SponsorPhone = a.SponsorPhone;
            SponsorAddress = a.SponsorAddress;
            CurrentSession = a.LastEnrollmentSession;
            CurrentStatus = a.LastEnrollmentStatus;
            CategoryId = a.StudentCategoryId;
            Programme = a.Program;
            //  Level = a.l;
            //  Category = a.CurrentPost;

        }
        public StudentPerson(IdentityDataLayer.DataModels.StudentPerson a, Dictionary<int, string> Fmds) : base(a.Person,Fmds)
        {

            Department = new Department();
            if (a.Department != null)
            {
                Department = new Department(a.Department.DepartmentCode, a.Department.DepartmentName,a.Department.CollegeCode);
            }
            Level = a.CurrentLevel;
            SponsorName = a.SponsorName;
            SponsorPhone = a.SponsorPhone;
            SponsorAddress = a.SponsorAddress;
            CurrentSession = a.LastEnrollmentSession;
            CurrentStatus = a.LastEnrollmentStatus;
            CategoryId = a.StudentCategoryId;
            Programme = a.Program;
            //  Level = a.l;
            //  Category = a.CurrentPost;

        }
    }
}
