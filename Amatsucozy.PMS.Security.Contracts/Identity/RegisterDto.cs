namespace Amatsucozy.PMS.Security.Contracts.Identity;

public sealed class RegisterDto
{
    public required string UserName { get; set; }
    
    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string ConfirmPassword { get; set; }
}