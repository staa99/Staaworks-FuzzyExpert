using System.Collections.Generic;

namespace Staaworks.BankExpert.Shared.Models
{
    public class Context
    {
        public Context ()
        {
            Questions = new List<Question>();
        }
        public string Name { get; set; }
        public string Declarations { get; set; }
        public List<Question> Questions { get; set; }
        public Output Output { get; set; }
    }
}
