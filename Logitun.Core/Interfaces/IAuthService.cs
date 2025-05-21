using Logitun.Core.Entities;
using Logitun.Shared.DTOs;

namespace Logitun.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> AuthenticateAsync(LoginDto request);
    string GenerateJwtToken(Credentials credentials);
    Task<bool> RegisterAsync(AuthRegisterRequest request);
    Task<IEnumerable<DriverDto>> GetAllDriversAsync();
    Task<IEnumerable<DriverDto>> GetAvailableDriversAsync();
    Task<bool> CreateAdminAccountAsync();
}
