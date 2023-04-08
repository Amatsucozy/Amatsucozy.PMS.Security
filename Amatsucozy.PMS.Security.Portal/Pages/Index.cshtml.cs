using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IndexModel(ILogger<IndexModel> logger, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _signInManager = signInManager;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage(_signInManager.IsSignedIn(User) ? "Account/Manage/Index" : "Account/Login");
    }
}