using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CanaryOverflow.Infrastructure.Data;

public class CanaryDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public CanaryDbContext(DbContextOptions<CanaryDbContext> options) : base(options)
    {
    }
}
