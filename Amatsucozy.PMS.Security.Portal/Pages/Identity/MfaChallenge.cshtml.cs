using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages.Identity;

public class MfaChallenge : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<Login> _logger;

    public MfaChallenge(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<Login> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;

        Input = new()
        {
            TwoFactorCode = string.Empty,
            RememberMachine = false
        };
    }

    [BindProperty]
    public MfaChallengeDto Input { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGet(bool rememberMe, string returnUrl)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user is null)
        {
            throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        }

        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user == null)
        {
            throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        }

        var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            authenticatorCode,
            rememberMe,
            Input.RememberMachine);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

            return RedirectToPage("./Lockout");
        }

        _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

        return Page();
    }
}