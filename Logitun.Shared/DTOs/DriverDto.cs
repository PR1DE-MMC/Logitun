namespace Logitun.Shared.DTOs;

public class DriverDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Cin { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public string Login { get; set; } = string.Empty;
}