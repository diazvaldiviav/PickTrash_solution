namespace WebApplication1.Models.Dtos.UserDto
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DefaultAddress { get; set; } = string.Empty;
    }

    public class DriverDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal Rating { get; set; }
    }
}
