using System;
using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.UserAggregate;
using CanaryOverflow.Infrastructure.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CanaryOverflow.Infrastructure.Contexts
{
    public class CanaryDbContext : DbContext, IDesignTimeDbContextFactory<CanaryDbContext>
    {
        public CanaryDbContext()
        {
        }

        public CanaryDbContext(DbContextOptions<CanaryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<AnswerComment> AnswerComments { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<QuestionComment> QuestionComments { get; set; } = null!;
        public DbSet<QuestionVote> QuestionVotes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new QuestionVoteEntityTypeConfiguration());
            base.OnModelCreating(builder);
        }

        public CanaryDbContext CreateDbContext(string[] args)
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