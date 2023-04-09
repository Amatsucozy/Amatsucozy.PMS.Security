using Microsoft.AspNetCore.Identity;

namespace Amatsucozy.PMS.Security.Core.Identity;

public sealed class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
