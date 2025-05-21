using BCrypt.Net;
using Logitun.Core.Entities;
using Logitun.Core.Interfaces;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Logitun.Shared.DTOs;

namespace Logitun.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto request)
    {
        var credentials = await _context.AuthCredentials
            .Include(c => c.Roles)
            .FirstOrDefaultAsync(c => c.Login == request.Username);

        if (credentials == null || !BCrypt.Net.BCrypt.Verify(request.Password, credentials.PasswordHash))
            return null;

        var token = GenerateJwtToken(credentials);
        return new AuthResponseDto
        {
            Token = token
        };
    }

    public string GenerateJwtToken(Credentials credentials)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, credentials.Login),
            new Claim(ClaimTypes.NameIdentifier, credentials.CredentialId.ToString())
        };

        claims.AddRange(credentials.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> RegisterAsync(AuthRegisterRequest request)
    {
        // Check if login or email already exists
        var existing = await _context.AuthCredentials.AnyAsync(c => c.Login == request.Login) ||
                       (request.Email != null && await _context.AuthInformation.AnyAsync(i => i.Email == request.Email));
        if (existing)
            return false;

        // Get or create ROLE_DRIVER
        var role = await _context.AuthRoles.FindAsync("ROLE_DRIVER");
        if (role == null)
        {
            role = new Role { Name = "ROLE_DRIVER" };
            _context.AuthRoles.Add(role);
        }

        // Create PersonalInformation
        var info = new PersonalInformation
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Birthdate = request.Birthdate,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Cin = request.Cin
        };

        _context.AuthInformation.Add(info);
        await _context.SaveChangesAsync(); // Need the InformationId

        // Create Credentials
        var credentials = new Credentials
        {
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Activated = true,
            LangKey = "fr",
            Roles = new List<Role> { role }
        };
        _context.AuthCredentials.Add(credentials);
        await _context.SaveChangesAsync(); // Need the CredentialsId

        // Create User
        var user = new User
        {
            InformationId = info.InformationId,
            CredentialsId = credentials.CredentialId
        };
        _context.AuthUsers.Add(user);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DriverDto>> GetAllDriversAsync()
    {
        var drivers = await _context.AuthUsers
            .Include(u => u.Credentials)
                .ThenInclude(c => c.Roles)
            .Include(u => u.Information)
            .Where(u => u.Credentials.Roles.Any(r => r.Name == "ROLE_DRIVER"))
            .Select(u => new DriverDto
            {
                UserId = u.UserId,
                FirstName = u.Information.FirstName,
                LastName = u.Information.LastName,
                Email = u.Information.Email ?? string.Empty,
                PhoneNumber = u.Information.PhoneNumber ?? string.Empty,
                Cin = u.Information.Cin ?? string.Empty,
                Birthdate = u.Information.Birthdate,
                Login = u.Credentials.Login
            })
            .ToListAsync();

        return drivers;
    }

    public async Task<IEnumerable<DriverDto>> GetAvailableDriversAsync()
    {
        var currentDate = DateTime.UtcNow.Date;

        // Get all drivers
        var allDrivers = await _context.AuthUsers
            .Include(u => u.Credentials)
                .ThenInclude(c => c.Roles)
            .Include(u => u.Information)
            .Where(u => u.Credentials.Roles.Any(r => r.Name == "ROLE_DRIVER"))
            .Select(u => new DriverDto
            {
                UserId = u.UserId,
                FirstName = u.Information.FirstName,
                LastName = u.Information.LastName,
                Email = u.Information.Email ?? string.Empty,
                PhoneNumber = u.Information.PhoneNumber ?? string.Empty,
                Cin = u.Information.Cin ?? string.Empty,
                Birthdate = u.Information.Birthdate,
                Login = u.Credentials.Login
            })
            .ToListAsync();

        // Get unavailable driver IDs (those with accepted time off requests that include current date)
        var unavailableDriverIds = await _context.TimeOffRequests
            .Where(t => t.Status == "ACCEPTED" &&
                       t.StartDate.Date <= currentDate &&
                       t.EndDate.Date >= currentDate)
            .Select(t => t.DriverId)
            .ToListAsync();

        // Filter out unavailable drivers
        return allDrivers.Where(d => !unavailableDriverIds.Contains(d.UserId));
    }
}
