using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CanaryOverflow.Identity.Data;

public class CanaryDbContextFactory : IDesignTimeDbContextFactory<CanaryDbContext>
{
    public CanaryDbContext CreateDbContext(string[] args)
    {
        if (args.Length < 1)
            throw new ArgumentException("The first argument must be a connection string.", nameof(args));

        var optionsBuilder = new DbContextOptionsBuilder<CanaryDbContext>();
        optionsBuilder.UseNpgsql(args[0]);

        return new CanaryDbContext(optionsBuilder.Options);
    }
}
