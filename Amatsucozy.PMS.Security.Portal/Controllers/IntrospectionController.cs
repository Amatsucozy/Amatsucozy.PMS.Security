using Amatsucozy.PMS.Shared.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amatsucozy.PMS.Security.Portal.Controllers;

public sealed class IntrospectionController: SecuredController
{
    [Authorize("Introspection")]
    [HttpGet]
    [Route("ping/[action]")]
    public IActionResult Introspection()
    {
        return Ok("pong");
    }

    [Authorize("Introspection1")]
    [Route("ping/[action]")]
    [HttpGet]
    public IActionResult Introspection1()
    {
        return Ok("pong");
    }
}
