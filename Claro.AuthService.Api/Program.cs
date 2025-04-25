using Azure.Identity;
using Claro.AuthService.Application.Helpers;
using Claro.AuthService.Application.Interfaces;
using Claro.AuthService.Application.Services;
using Claro.AuthService.Domain.Dtos;
using Claro.AuthService.Infrastructure.Persistence;
using Claro.AuthService.Infrastructure.Repositories;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// 1. Configure Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// 2. (Optional but best) Configure ApplicationInsights manually if needed
builder.Services.Configure<TelemetryConfiguration>(config =>
{
    config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

//add swagger service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container
var jwtSettings = builder.Configuration.GetSection("keyVaultSettings_JWT");

var keyVaultName = jwtSettings["keyVaultName"];

if (string.IsNullOrWhiteSpace(keyVaultName))
{
    throw new InvalidOperationException("KeyVault name is not set. Please check your configuration.");
}

var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(kvUri, new DefaultAzureCredential());

builder.Services.Configure<JwtSettings>(options =>
{
    options.Key = builder.Configuration["JwtKey"];
    options.Issuer = builder.Configuration["JwtIssuer"];
    options.Audience = builder.Configuration["JwtAudience"];
    options.ExpiryMinutes = int.Parse(builder.Configuration["JwtExpiryMinutes"]);
});

// Add Identity services 
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//                .AddEntityFrameworkStores<ClaroAuthDbContext>()
//                .AddDefaultTokenProviders();

var jwtKey = Encoding.ASCII.GetBytes(builder.Configuration["JwtKey"]);
var jwtIssuer = builder.Configuration["JwtIssuer"];
var jwtAudience = builder.Configuration["JwtAudience"];


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ClaroAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<Claro.AuthService.Infrastructure.Interfaces.IUserRepository, UserRepository>();  


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClaroUI",
        policy => policy.WithOrigins("http://localhost:4200") // Update to your Angular domain
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowClaroUI");

app.UseSwagger();
    app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}
//app.MapOpenApi();

app.UseHttpsRedirection();

app.MapControllers();

try
{
    // all your config and service setup
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Startup failed: " + ex.Message);
    throw;
}

