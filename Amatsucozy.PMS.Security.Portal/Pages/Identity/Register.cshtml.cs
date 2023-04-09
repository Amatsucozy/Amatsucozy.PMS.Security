using System.Collections.Immutable;
using System.Text;
using System.Text.Encodings.Web;
using Amatsucozy.PMS.Security.Contracts.Identity;
using Amatsucozy.PMS.Security.Core.Identity;
using Amatsucozy.PMS.Security.Portal.Pages.Shared;
using Amatsucozy.PMS.Security.Portal.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Amatsucozy.PMS.Security.Portal.Pages.Identity;

public class Register : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly ILogger<Register> _logger;
    private readonly IEmailSendRequestBuilder _emailSendRequestBuilder;
    private readonly IPublishEndpoint _publishEndpoint;

    public Register(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IUserStore<User> userStore,
        ILogger<Register> logger,
        IEmailSendRequestBuilder emailSendRequestBuilder,
        IPublishEndpoint publishEndpoint)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _logger = logger;
        _emailSendRequestBuilder = emailSendRequestBuilder;
        _publishEndpoint = publishEndpoint;

        Input = new()
        {
            UserName = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            ConfirmPassword = string.Empty
        };
        ExternalLogins = ImmutableList<AuthenticationScheme>.Empty;
    }

    [BindProperty]
    public RegisterDto Input { get; set; }

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public async Task OnGetAsync(string returnUrl)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl)
    {
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid) return BadRequest();

        var user = new User();

        await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Identity",
                new ConfirmEmailDto
                {
                    UserId = userId,
                    Code = code,
                    ReturnUrl = returnUrl
                },
                protocol: Request.Scheme);

            await _publishEndpoint.Publish(
                _emailSendRequestBuilder.GetConfirmEmailSendRequest(
                    Input.Email,
                    Input.UserName,
                    HtmlEncoder.Default.Encode(callbackUrl ?? string.Empty),
                    "Amatsucozy"
                ));

            return RedirectToPage("/Identity/Result", new Result
            {
                Title = "Confirm your email",
                Message = "Thank you for registering. Before you can sign in, please confirm your email address by clicking on the link we just emailed you."
            });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    private IUserEmailStore<User> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<User>)_userStore;
    }
}