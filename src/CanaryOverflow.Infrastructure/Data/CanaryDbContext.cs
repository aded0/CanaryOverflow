using CanaryOverflow.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CanaryOverflow.Infrastructure.Data;

public class CanaryDbContext : IdentityDbContext<User, Role, Guid>
{
    public CanaryDbContext(DbContextOptions<CanaryDbContext> options) : base(options)
    {
    }
}
