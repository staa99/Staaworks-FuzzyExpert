using IdentityCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCapturingClient.ViewModels
{
    public class SearchViewModel
    {
        public string PersonID { get; set; }
        public string Name {get; set;}
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Sex { get ; set ; }
        public string NextOfKinName { get; set; }
    }
    
}
