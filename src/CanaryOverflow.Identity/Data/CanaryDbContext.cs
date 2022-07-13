using CanaryOverflow.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CanaryOverflow.Identity.Data;

public class CanaryDbContext : IdentityDbContext<User, Role, Guid>
{
    public CanaryDbContext(DbContextOptions<CanaryDbContext> options) : base(options)
    {
    }
}
