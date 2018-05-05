using System.Windows.Forms;
using FIngerCapturing.Common;
using Staaworks.BankExpert.Shared.Interfaces;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;

namespace Staaworks.BankExpert.WinForms.Registration
{
    class UserCreatorData : IViewDataHolder
    {
        public IDataReciever Reciever { get; }
        public UserCreatorData(IDataReciever reciever)
        {
            Reciever = reciever;
            FingerprintData = new UserCreatorFingerprintData
            {
                MainObject = new MainObject()
            };
            FaceData = new UserCreatorFaceData
            {
                RecognitionSchemeData = new RecognitionSchemeData()
            };

            FaceData.RecognitionForm = new FaceRecognition.RecognitionForm(5, (FaceRecognitionSchemes.RegistrationScheme, FaceData.RecognitionSchemeData));
        }

        public User User { get; set; }
        public UserCreatorFingerprintData FingerprintData { get; set; }
        public UserCreatorFaceData FaceData { get; set; }
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
}
