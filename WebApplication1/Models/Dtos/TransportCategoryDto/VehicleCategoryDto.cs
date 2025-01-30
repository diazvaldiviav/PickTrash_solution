namespace WebApplication1.Models.Dtos.TransportCategory
{
    public class TransportCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal PricePerKm { get; set; }
    }
}
