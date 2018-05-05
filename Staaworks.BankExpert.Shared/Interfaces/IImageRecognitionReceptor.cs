namespace Staaworks.BankExpert.Shared.Interfaces
{
    public interface IImageRecognitionReceptor
    {
        FaceRecognitionSchemes RecognitionScheme { get; }
        int GetResolvedLabel ();
    }

    public enum FaceRecognitionSchemes
    {
        RegistrationScheme, AuthenticationScheme
    }
}
