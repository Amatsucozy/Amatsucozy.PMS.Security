namespace Amatsucozy.PMS.Security.Contracts.Identity;

public sealed class MfaChallengeDto
{
    public required string TwoFactorCode { get; set; }

    public bool RememberMachine { get; set; }
}