namespace Staaworks.BankExpert.Shared.Models
{
    public class Option
    {
        public string UserText { get; set; }
        public float Value { get; set; }
        public OptionType Type { get; set; }
    }


    public enum OptionType
    {
        freetext, constant
    }
}
