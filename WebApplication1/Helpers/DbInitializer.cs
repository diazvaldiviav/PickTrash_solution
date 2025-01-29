using WebApplication1.Data;
using WebApplication1.Models.Domain;

namespace WebApplication1.Helpers
{
    // Data/SeedData/DbInitializer.cs
    public static class DbInitializer
    {
        public static async Task SeedData(PickTrashDbContext context)
        {
            if (context.TransportCategories.Any())
                return; // La base de datos ya tiene datos

            var categories = new List<TransportCategory>
        {
            new TransportCategory
            {
                Name = "Sedan",
                Description = "Vehículo de pasajeros con maletero separado",
                MinWeight = 1200,
                MaxWeight = 2000
            },
            new TransportCategory
            {
                Name = "SUV",
                Description = "Vehículo deportivo utilitario",
                MinWeight = 2001,
                MaxWeight = 3000
            },
            new TransportCategory
            {
                Name = "Pickup",
                Description = "Camioneta con área de carga descubierta",
                MinWeight = 3001,
                MaxWeight = 4000
            },
            new TransportCategory
            {
                Name = "Van",
                Description = "Furgoneta para carga y transporte",
                MinWeight = 2500,
                MaxWeight = 3500
            },
            new TransportCategory
            {
                Name = "Light Truck",
                Description = "Camión ligero para distribución urbana",
                MinWeight = 4001,
                MaxWeight = 7500
            },
            new TransportCategory
            {
                Name = "Medium Truck",
                Description = "Camión mediano para transporte regional",
                MinWeight = 7501,
                MaxWeight = 12000
            },
            new TransportCategory
            {
                Name = "Heavy Truck",
                Description = "Camión pesado para transporte de gran volumen",
                MinWeight = 12001,
                MaxWeight = 26000
            }
        };

            await context.TransportCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
