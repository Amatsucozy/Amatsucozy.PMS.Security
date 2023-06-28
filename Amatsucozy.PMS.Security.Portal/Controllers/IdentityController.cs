using System.Security.Claims;
using System.Text;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core.Identity;
using Amatsucozy.PMS.Shared.API.Controllers;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

public sealed class IdentityController : PublicController
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IIssuerNameService _issuerNameService;
    private static readonly HashSet<string> ViewScopes = new()
    {
        "sts",
        "accounts",
        "pms"
    };

    public IdentityController(UserManager<User> userManager, ITokenService tokenService, IIssuerNameService issuerNameService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _issuerNameService = issuerNameService;
    }

    [HttpGet(Name = nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{model.UserId}'.");
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ConfirmEmailAsync(user, code);

        return RedirectToPage("/Index");
    }

    [Authorize]
    [Route("[action]")]
    [HttpGet]
    public IActionResult GetAvailablePersonalAccessTokenScopes()
    {
        return Ok(ViewScopes);
    }

    [Authorize]
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
