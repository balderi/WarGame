namespace WarGame.Models.Auth
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public Guid BaseId { get; set; }
    }
}
