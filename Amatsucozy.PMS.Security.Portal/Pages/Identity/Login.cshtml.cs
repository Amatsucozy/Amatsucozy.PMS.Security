using System.Collections.Immutable;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages.Identity;

public class Login : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<Login> _logger;

    public Login(SignInManager<User> signInManager, ILogger<Login> logger)
    {
        _signInManager = signInManager;
        _logger = logger;

        Input = new()
        {
            Email = string.Empty,
            Password = string.Empty,
            RememberMe = false
        };
        ExternalLogins = ImmutableList<AuthenticationScheme>.Empty;
    }

    [BindProperty]
    public LoginDto Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }


    public async Task OnGetAsync(string returnUrl)
    {
        if (_signInManager.IsSignedIn(User))
        {
            RedirectToPage("/Index");
            return;
        }

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl)
    {
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa",
                new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}