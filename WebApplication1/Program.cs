using Microsoft.EntityFrameworkCore;
using WebApplication1.Common.Middleware;
using WebApplication1.Data;
using WebApplication1.Helpers;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Services.Implementations;
using WebApplication1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Serilog;
using WebApplication1.Data.Repositories.Implementations;
using WebApplication1.Data.Repositories.Interfaces;



// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/picktrash_.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Agregar Serilog al builder
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PickTrashDbContext>(options =>
    options.UseInMemoryDatabase("PickTrashDb"));



//authenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


//Mappeo de entidades
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


//injeection dependencies
builder.Services.AddScoped<IAuthServices, AuthServices>();



// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITransportCategoryRepository, TransportCategoryRepository>();




var app = builder.Build();

// Sembrar datos de prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PickTrashDbContext>();
    await DbInitializer.SeedData(context);
}

//middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
