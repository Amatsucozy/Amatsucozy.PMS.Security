using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amatsucozy.PMS.Security.Infrastructure;

public sealed class SecurityDbContext : IdentityDbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
}