using IdentityBusinessLayer.Models;
using IdentityBusinessLayer.Repositories;
using IdentityCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCapturingClient.ViewModels
{
    public class ProfileViewModel
    {
        public string PersonId { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Deptment { get; set; }
        public string Level { get; set; }
        public string Programme { get; set; }
        public string Email { get; set; }
        public string SponsorName { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorAddress { get; set; }
        public string Designation { get; set; }
        public string College { get; set; }
        public string HealthId { get; set; }
        public string BloodGrp { get; set; }
        public string Phone { get; set; }
        public int categoryid { get; set; }
        public string Address { get; set; }
        public char Gender { get; set; }
        public string Next_of_KinName { get; set; }
        public string Next_of_KinPhone { get; set; }
        public string Next_of_KinAddress { get; set; }
        public byte [] Photo { get; set; }
        public Dictionary<int, string> Fmds { get; set; }
        // public int? currentsnapshotid { get; set; }
        StudentPerson sp = null;
        public ProfileViewModel()
        {

        }
        public ProfileViewModel(SearchViewModel SelectedItem,bool updatefromsource)
        {
            if (SelectedItem != null)
            {
                SearchPeople repo = new SearchPeople();
                sp = repo.filterStudentPersonById(SelectedItem.PersonID, updatefromsource);
                this.PersonId = sp.PersonID;
                this.Surname = sp.Surname;
                this.Firstname = sp.Firstname;
                this.Middlename = sp.Middlename;
                this.Deptment = sp.Department.Code;
                this.College = sp.Department.CollegeCode;
                this.Email = sp.Email;
                this.SponsorName = sp.SponsorName;
                this.SponsorPhone = sp.SponsorPhone;
                this.SponsorAddress = sp.SponsorAddress;
                this.Level = sp.Level;
                this.Programme = sp.Programme;
                this.HealthId = sp.HCN;
                this.BloodGrp = sp.BG;
                this.Phone = sp.Phone;
                this.Address = sp.Address;
                this.Gender = sp.Sex;
                this.Next_of_KinName = sp.NextOfKin.Surname + " " + sp.NextOfKin.Firstname;
                this.Next_of_KinPhone = sp.NextOfKin.Phone;
                this.Next_of_KinAddress = sp.NextOfKin.Address;
                this.Photo = sp.Photo;
                this.categoryid = sp.CategoryId;
                this.Fmds = sp.Fmds;
               // this.currentsnapshotid = null;
            }

        }

        public bool saveChanges()
        {
            if (this.sp == null) return false;
            updateperson();
            PersonRepository repo = new PersonRepository();

           return  repo.saveStudentPersonProfile(sp.PersonID,sp, null);
        }

        private void updateperson()
        {
            // sp.PersonID = this.PersonId;
            sp.Surname=this.Surname;
            sp.Firstname=this.Firstname ;
            sp.Middlename=this.Middlename ;
            sp.Department.Code=this.Deptment ;
            sp.Department.CollegeCode= this.College ;
            sp.Email=this.Email;
            sp.SponsorName=this.SponsorName;
            sp.SponsorPhone=this.SponsorPhone ;
            sp.SponsorAddress=this.SponsorAddress ;
            sp.Level=this.Level;
            sp.HCN=this.HealthId;
            sp.BG= this.BloodGrp ;
            sp.Phone=this.Phone ;
            sp.Address=this.Address;
            sp.Sex=this.Gender ;
            //this.Next_of_KinName = sp.NextOfKin.Surname + " " + sp.NextOfKin.Firstname;
           // this.Next_of_KinPhone = sp.NextOfKin.Phone;
           // this.Next_of_KinAddress = sp.NextOfKin.Address;
           // this.Photo = sp.Photo;
           // this.Fmds = sp.Fmds;
        }
    }

}
