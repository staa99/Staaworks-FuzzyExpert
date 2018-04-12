using System.Collections.Generic;

namespace Staaworks.BankExpert.Shared.Models
{
    public class Output
    {
        public Output()
        {
            Definitions = new List<OutputDefinition>();
        }
        public string Name { get; set; }
        public DefinitionType Type { get; set; }
        public List<OutputDefinition> Definitions { get; set; }
    }


    public enum DefinitionType
    {
        overflow, constant
    }
}
