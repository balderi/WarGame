namespace WarGame.Models.Auth
{
    public class UserData
    {
#pragma warning disable CS8618 //Stop whining about nullable types!
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
