using System.Security.Claims;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core;
using Amatsucozy.PMS.Shared.API.Controllers;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

public sealed class TokenController : SecuredController
{
    private readonly ITokenService _tokenService;
    private readonly IIssuerNameService _issuerNameService;
    private readonly IdentityServerTools _identityServerTools;

    private static readonly HashSet<string> ViewScopes = new()
    {
        "sts",
        "accounts",
        "pms"
    };

    public TokenController(ITokenService tokenService, IIssuerNameService issuerNameService, IdentityServerTools identityServerTools)
    {
        _tokenService = tokenService;
        _issuerNameService = issuerNameService;
        _identityServerTools = identityServerTools;
    }

    [Route("[action]")]
    [HttpGet]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
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
    
    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> GenerateAccessToken(PatOptions patOptions)
    {
        var test = patOptions.AllowedScopes.Where((x) => ViewScopes.Contains(x)).Select(x => new Claim(nameof(x), x));

        var issuer = "https://localhost:60002";
        var token = await _identityServerTools.IssueJwtAsync(
            30000,
            issuer, new List<Claim>()
            {
                new("client_id", "pms-ui"),
                // new("sub", User.GetSubjectId()),
                // new("sts", "sts"),
                // new("accounts", "accounts"),
                // new("pms", "pms"),
            }
        );
        return Ok(token);
    }
}
