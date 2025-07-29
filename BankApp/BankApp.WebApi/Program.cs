using System.Text;
using BankApp.Application.Interfaces;
using BankApp.Infrastructure.Data;
using BankApp.Infrastructure.Repositories;
using BankApp.WebApi.Handlers.People;
using BankApp.WebApi.HttpClients.Compliance;
using BankApp.WebApi.Services.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var complianceApiUrl = builder.Configuration["ExternalApis:ComplianceApi"] ?? throw new Exception("Configure ExternalApis:ComplianceApi");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Configure Jwt:Issuer");
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Configure Jwt:Key");
var complianceAuthApiUrl = complianceApiUrl;

builder.Services.AddRefitClient<IComplianceApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(complianceApiUrl));

builder.Services.AddRefitClient<IComplianceAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(complianceAuthApiUrl));

builder.Services.AddSingleton<ComplianceAuthService>();
builder.Services.AddDbContext<BankAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CreatePersonHandler>();
builder.Services.AddScoped<AuthenticatePersonHandler>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();