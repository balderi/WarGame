namespace WarGame.Models.Auth
{
    public class LoginResultDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
