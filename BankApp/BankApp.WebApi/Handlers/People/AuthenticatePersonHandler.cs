using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BankApp.WebApi.DTOs.People;

namespace BankApp.WebApi.Handlers.People;

public class AuthenticatePersonHandler(
    ICustomerRepository customerRepository,
    IConfiguration configuration)
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthenticationResponse> HandleAsync(AuthenticatePersonCommand request)
    {
        var customer = await _customerRepository.GetByEmailAsync(request.Email) ?? throw new InvalidOperationException("Credenciais inválidas.");
        var passwordHasher = new PasswordHasher<Customer>();
        var result = passwordHasher.VerifyHashedPassword(customer, customer.Password, request.Password);
        if (result != PasswordVerificationResult.Success)
            throw new InvalidOperationException("Credenciais inválidas.");

        var token = GenerateJwtToken(customer);
        return new AuthenticationResponse { AccessToken = token };
    }

    private string GenerateJwtToken(Customer customer)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer))
            throw new InvalidOperationException("Configuração de JWT inválida.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, customer.Email),
            new Claim(ClaimTypes.Name, customer.Name),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtIssuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}