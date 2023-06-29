using System.Security.Claims;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Shared.API.Controllers;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

[Authorize("ApiScope")]
public sealed class TokenController : SecuredController
{
    private readonly ITokenService _tokenService;
    private readonly IIssuerNameService _issuerNameService;

    private static readonly HashSet<string> ViewScopes = new()
    {
        "sts",
        "accounts",
        "pms"
    };

    public TokenController(ITokenService tokenService, IIssuerNameService issuerNameService)
    {
        _tokenService = tokenService;
        _issuerNameService = issuerNameService;
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult GetAvailablePersonalAccessTokenScopes()
    {
        return Ok(ViewScopes);
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> GeneratePersonalAccessToken(PatOptions patOptions)
    {
        var token = new Token(IdentityServerConstants.TokenTypes.AccessToken)
        {
            Issuer = await _issuerNameService.GetCurrentAsync(),
            Lifetime = 300,
            CreationTime = DateTime.UtcNow,
            ClientId = "pat.client",
            Claims = new List<Claim>
            {
                new("client_id", "pat.client"),
                new("sub", User.GetSubjectId()),
            },
            AccessTokenType = AccessTokenType.Reference
        };

        foreach (var scope in patOptions.AllowedScopes)
        {
            if (ViewScopes.Contains(scope))
            {
                token.Claims.Add(new Claim(nameof(scope), scope));
            }
        }

        return Ok(await _tokenService.CreateSecurityTokenAsync(token));
    }
}
