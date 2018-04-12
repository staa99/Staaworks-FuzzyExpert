using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCommons.Models
{
    public class StaffPerson : Person
    {
     
        public Department Department { get; set; }
        public String Designation { get; set; }
        public String Post { get; set; }
        public String Grade { get; set; }
        public String Level { get; set; }
        public StaffPerson()
        {

        }
        public StaffPerson(IdentityDataLayer.DataModels.Person a):base(a)
        {
           
           
           
        }
        public StaffPerson(IdentityDataLayer.DataModels.StaffPerson a):base(a.Person)
        {
         
            Department = new Department();
            if (a.Department != null)
            {
                Department = new Department(a.Department.DepartmentCode,a.Department.DepartmentName,a.Department.CollegeCode);
            }
            Designation = a.CurrentDesignation;
            Post = a.CurrentPost;

        }
    }
}
