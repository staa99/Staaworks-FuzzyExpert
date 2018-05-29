namespace Staaworks.BankExpert.Shared.Models
{
    public class UserEntity: User
    {
        public int Id { get; set; }


        public static UserEntity CreateFromUser (User user) => new UserEntity
        {
            Address = user.Address,
            Email = user.Email,
            HashSalt = user.HashSalt,
            ImageLocation = user.ImageLocation,
            Name = user.Name,
            PasswordHash = user.PasswordHash,
            Phone = user.Phone,
            ZipOrPostalAddress = user.ZipOrPostalAddress,
            Snapshots = user.Snapshots
        };
    }
}
