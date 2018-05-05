using System;

namespace Staaworks.BankExpert.Shared.Interfaces
{
    public interface IImageRecognitionReceptor
    {

        (FaceRecognitionSchemes scheme, object data) SchemeInfo { get; }
        string GetResolvedLabel ();
        Action OnComplete { get; }
    }

    public enum FaceRecognitionSchemes
    {
        RegistrationScheme, AuthenticationScheme
    }


    public class RecognitionSchemeData
    {
        public string Email { get; set; }
        public bool Successful { get; set; }
    }
}
