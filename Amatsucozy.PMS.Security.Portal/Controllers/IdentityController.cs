using System.Text;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core.Identity;
using Amatsucozy.PMS.Shared.API.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

public sealed class IdentityController : PublicController
{
    private readonly UserManager<User> _userManager;

    public IdentityController(UserManager<User> userManager)
    {
        _userManager = userManager;
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
}