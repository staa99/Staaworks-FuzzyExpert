using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCommons.Models
{
   public  interface IPerson
    {
        string PersonID { get; set; }
        string Surname { get; set; }
        string Firstname { get; set; }
        string Middlename { get; set; }
        string Address { get; set; }
        char Sex { get; set; }
        string HCN { get; set; }
        string BG { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        IPerson NextOfKin { get; set; }
        byte[] Photo { get; set; }
        Dictionary<int, string> Fmds { get; set; }
    }

    public class Person : IPerson
    {
        public string PersonID { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Address { get; set; }
        public char Sex { get; set; }
        public string HCN { get; set; }
        public string BG { get; set; }
        public string Phone { get; set; }
        public IPerson NextOfKin { get; set; }
        public byte[] Photo { get; set; }
        public Dictionary<int, string> Fmds { get; set; }
        public string Email { get; set; }


        public Person()
        {

        }
        public Person(IdentityDataLayer.DataModels.Person a)
        {
            PersonID = a.IdentityNo;
            Surname = a.Surname;
            Firstname = a.Firstname;
            Address = a.Address;
            Middlename = a.Middlename;
            Phone = a.Phone;
            Sex = String.IsNullOrEmpty(a.Sex)?' ':a.Sex[0];
            BG = a.BloodGroup;
            HCN = a.HealthCentreNo;
            NextOfKin = new Person();
            Photo = a.CurrentPhoto;
            Email = a.Email;
            
        }
        public Person(IdentityDataLayer.DataModels.Person a, Dictionary<int, string> Fmds):this(a)
        {
            this.Fmds = Fmds;
        }


    }

    public class Department
    {
        public Department()
        {

        }
        public Department(string code, string name,string collegecode)
        {
            Code = code;
            Name = name;
            CollegeCode = collegecode;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CollegeCode { get; set; }
    }
    public enum persontype { ALL, STAFF, STUDENT, GENERAL }
}
