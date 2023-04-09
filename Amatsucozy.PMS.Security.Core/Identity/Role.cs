using Microsoft.AspNetCore.Identity;

namespace Amatsucozy.PMS.Security.Core.Identity;

public sealed class Role : IdentityRole<Guid>
{
    public Role()
    {
        Id = Guid.NewGuid();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
