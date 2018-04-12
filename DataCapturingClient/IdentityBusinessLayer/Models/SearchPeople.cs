using IdentityBusinessLayer.Repositories;
using IdentityCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityBusinessLayer.Models
{
    public class SearchPeople
    {
        // PersonRepository repo
        PersonRepository person_repo;
        public SearchPeople()
        {
         person_repo = new PersonRepository();
        }

        public List<Person> filterPeople(String id, String surname, String firstanme, persontype persontype = persontype.ALL)
        {
            //check config for updating from sources
            bool updatefromsource = true; ;

            List<Person> results = person_repo.getPersons(id, surname, firstanme, persontype, updatefromsource);
            //treat results
            return results;
            
        }
        public StaffPerson filterStaffPersonById(String id)
        {
            //check config for updating from source
            bool updatefromsource = true;

            StaffPerson results = person_repo.getStaffPersonById(id);
            //treat results
            return results;

        }
        public StudentPerson filterStudentPersonById(String id, bool updatefromsource=false)
        {
            //check config for updating from source
           // bool updatefromsource = true;

            StudentPerson results = person_repo.getStudentPersonById(id, updatefromsource);
            //treat results
            return results;

        }
    }
}
