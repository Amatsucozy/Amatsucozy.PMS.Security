// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amatsucozy.PMS.Security.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages.Account.Manage;

[Authorize]
public class PersonalDataModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<PersonalDataModel> _logger;

    public PersonalDataModel(
        UserManager<User> userManager,
        ILogger<PersonalDataModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        return Page();
    }
}
