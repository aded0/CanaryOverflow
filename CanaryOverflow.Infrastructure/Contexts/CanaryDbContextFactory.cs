using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CanaryOverflow.Infrastructure.Contexts
{
    public class CanaryDbContextFactory : IDesignTimeDbContextFactory<CanaryDbContext>
    {
        CanaryDbContext IDesignTimeDbContextFactory<CanaryDbContext>.CreateDbContext(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Single argument must be a connection string.", nameof(args));
            }
        
            var optionsBuilder = new DbContextOptionsBuilder<CanaryDbContext>();
            optionsBuilder.UseNpgsql(args[0]);
        
            return new CanaryDbContext(optionsBuilder.Options);
        }
    }
}