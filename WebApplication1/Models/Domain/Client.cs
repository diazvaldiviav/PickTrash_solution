namespace WebApplication1.Models.Domain
{
    public class Client
    {
        public int Id { get; set; }
        public string DefaultAddress { get; set; } = string.Empty;

        // Relación con User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
