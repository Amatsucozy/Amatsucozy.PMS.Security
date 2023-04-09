using Amatsucozy.PMS.Security.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly SignInManager<User> _signInManager;

    public IndexModel(ILogger<IndexModel> logger, SignInManager<User> signInManager)
    {
        _logger = logger;
        _signInManager = signInManager;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage(_signInManager.IsSignedIn(User) ? "Account/Manage/Index" : "Identity/Login");
    }
}