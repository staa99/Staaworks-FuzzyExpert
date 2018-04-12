using System.Collections.Generic;
namespace Staaworks.BankExpert.Shared.Models
{
    public class Question
    {
        public string Name { get; set; }
        public string UserText { get; set; }
        public string Source { get; set; }
        public List<Option> Options { get; set; }
    }
}
