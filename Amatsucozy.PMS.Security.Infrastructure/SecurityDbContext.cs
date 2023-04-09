using Amatsucozy.PMS.Security.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amatsucozy.PMS.Security.Infrastructure;

public sealed class SecurityDbContext : IdentityDbContext<User, Role, Guid>
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("security");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureMarker).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
