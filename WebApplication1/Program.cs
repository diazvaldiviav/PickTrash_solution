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
using Microsoft.OpenApi.Models;



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

//unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//geolocation
builder.Services.AddScoped<IGeocodingService, GoogleGeocodingService>();


//injeection dependencies
builder.Services.AddScoped<IAuthServices, AuthService>();
builder.Services.AddScoped<IUserServices, UserService>();
builder.Services.AddScoped<IVehicleServices, VehicleService>();
builder.Services.AddScoped<ITransportCategory, TransportCategoryService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IRequestService, RequestService>();



// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITransportCategoryRepository, TransportCategoryRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()       // Permite cualquier origen
                .AllowAnyMethod()       // Permite cualquier método HTTP (GET, POST, etc.)
                .AllowAnyHeader();      // Permite cualquier encabezado
        });
});


builder.Services.AddHttpClient();



//configuracion de roles y politicas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireDriverRole", policy =>
        policy.RequireRole("Driver"));

    options.AddPolicy("RequireClientRole", policy =>
        policy.RequireRole("Client"));
});

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PickTrash API",
        Version = "v1",
        Description = "API para el sistema de recolección de basura PickTrash"
    });

    // Configuración de seguridad para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                      "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});




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
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
