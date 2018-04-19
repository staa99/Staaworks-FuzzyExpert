using System.Windows.Forms;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;

namespace Staaworks.BankExpert.WinForms.Registration
{
    public class UserCreatorData : IViewDataHolder
    {
        public IDataReciever Reciever { get; }
        public UserCreatorData(IDataReciever reciever)
        {
            int a = 1000000000;
            Reciever = reciever;
            BioRegData = new BioRegData
            {
                FingerprintData = new UserCreatorFingerprintData
                {
                    MainObject = new FIngerCapturing.Common.MainObject()
                }
            };
        }

        public User User { get; set; }
        public BioRegData BioRegData { get; set; }
        public UserCreator_BasicForm CreatorForm { get; set; }
        public Control ViewDataControl { get => CreatorForm; set => CreatorForm = value as UserCreator_BasicForm; }

        public void PopulateControl(params object[] parameters)
        {
            CreatorForm = new UserCreator_BasicForm
            {
                CreatorData = this
            };
            Reciever.AddControl(CreatorForm);
        }
    }


    public class BioRegData
    {
        public UserCreatorFingerprintData FingerprintData { get; set; }
    }
}
