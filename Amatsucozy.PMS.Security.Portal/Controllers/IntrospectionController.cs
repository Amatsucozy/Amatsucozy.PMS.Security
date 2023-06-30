using Amatsucozy.PMS.Shared.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

[Authorize("ReferenceToken")]
public sealed class IntrospectionController: PublicController
{
    [Route("[action]")]
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}
