namespace WebApplication1.Models.Domain
{
    public class TransportCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }

        // Relación con Vehicle
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
