namespace Amatsucozy.PMS.Security.Contracts.Identity;

public sealed class PatOptions
{
    public int Timespan { get; set; }

    public string[] AllowedScopes { get; set; } = Array.Empty<string>();
}
