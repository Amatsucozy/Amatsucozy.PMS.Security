using Duende.IdentityServer.Models;

namespace Amatsucozy.PMS.Security.Core;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
    {
        new("sts")
        {
            DisplayName = "STS",
            Scopes =
            {
                "sts"
            },
            ApiSecrets = new List<Secret>
            {
                new("442B632E-1341-4643-9883-BC4C24395582".Sha256())
            }
        },
        new("accounts")
        {
            DisplayName = "Accounts",
            Scopes =
            {
                "accounts"
            },
            ApiSecrets = new List<Secret>
            {
                new("442B632E-1341-4643-9883-BC4C24395582".Sha256())
            }
        },
        new("pms")
        {
            DisplayName = "PMS",
            Scopes =
            {
                "pms"
            },
            ApiSecrets = new List<Secret>
            {
                new("442B632E-1341-4643-9883-BC4C24395582".Sha256())
            }
        },
        new("external")
        {
            DisplayName = "External",
            Scopes =
            {
                "accounts",
                "pms"
            },
            ApiSecrets = new List<Secret>
            {
                new("442B632E-1341-4643-9883-BC4C24395582".Sha256())
            }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new(name: "sts", displayName: "STS"),
            new(name: "accounts", displayName: "Accounts"),
            new(name: "pms", displayName: "PMS")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "pms-ui",
                ClientSecrets = new List<Secret>
                {
                    new("9D1D28FE-5899-48F2-9B6F-F1D0B8E3EE22".Sha256())
                },
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes =
                {
                    "openid",
                    "profile",
                    "sts",
                    "accounts",
                    "pms"
                },
                RedirectUris =
                {
                    "https://localhost:4200",
                    "https://localhost:4200/challenge",
                },
                PostLogoutRedirectUris =
                {
                    "https://localhost:4200"
                },
                AllowedCorsOrigins =
                {
                    "https://localhost:4200"
                },
                AccessTokenType = AccessTokenType.Jwt,
                RequireClientSecret = false
            },
            new()
            {
                ClientId = "pat.client",
                ClientSecrets =
                {
                    new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes =
                {
                    "sts",
                    "accounts",
                    "pms"
                }
            }
        };
}
