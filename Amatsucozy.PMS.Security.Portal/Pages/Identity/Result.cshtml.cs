using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amatsucozy.PMS.Security.Portal.Pages.Identity;

public class Result : PageModel
{
    public required string Title { get; set; }

    public required string Message { get; set; }
}