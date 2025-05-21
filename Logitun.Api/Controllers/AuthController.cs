using Logitun.Core.Interfaces;
using Logitun.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Logitun.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var response = await _authService.AuthenticateAsync(request);
        if (response == null) return Unauthorized("Invalid credentials");

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        var success = await _authService.RegisterAsync(request);
        if (!success) return Conflict("Login or email already exists.");

        return Ok("User registered successfully with role ROLE_DRIVER.");
    }

    
    [HttpGet("drivers")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAllDrivers()
    {
        var drivers = await _authService.GetAllDriversAsync();
        return Ok(drivers);
    }

    [HttpGet("drivers/available")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAvailableDrivers()
    {
        var drivers = await _authService.GetAvailableDriversAsync();
        return Ok(drivers);
    }

    [HttpPost("seed-admin")]
    public async Task<IActionResult> CreateAdminAccount()
    {
        var success = await _authService.CreateAdminAccountAsync();
        if (!success) return Conflict("Admin account already exists.");

        return Ok("Admin account created successfully.");
    }
}
