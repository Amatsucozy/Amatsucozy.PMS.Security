namespace Amatsucozy.PMS.Security.Contracts.Identity;

public sealed class ConfirmEmailDto
{
    public required string UserId { get; set; }
    
    public required string Code { get; set; }
    
    public required string ReturnUrl { get; set; }
}