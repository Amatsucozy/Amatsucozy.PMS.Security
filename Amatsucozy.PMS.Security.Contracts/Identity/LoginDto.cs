using System.ComponentModel.DataAnnotations;

namespace Amatsucozy.PMS.Security.Contracts.Identity;

public sealed class LoginDto
{
    public required string Email { get; set; }

    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}
