using BankApp.Application.Interfaces;
using BankApp.Infrastructure.Data;
using BankApp.Infrastructure.Repositories;
using BankApp.WebApi.HttpClients.Compliance;
using BankApp.WebApi.Persistence;
using BankApp.WebApi.Services.Compliance;
using Microsoft.EntityFrameworkCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var complianceApiUrl = builder.Configuration["ExternalApis:ComplianceApi"] ?? throw new Exception("Configure ExternalApis:ComplianceApi");
var complianceAuthApiUrl = complianceApiUrl;

builder.Services.AddRefitClient<IComplianceApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(complianceApiUrl));

builder.Services.AddRefitClient<IComplianceAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(complianceAuthApiUrl));

builder.Services.AddSingleton<ComplianceAuthService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<BankAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();