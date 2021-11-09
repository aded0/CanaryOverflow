using System;
using CanaryOverflow.Domain.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CanaryOverflow.Domain.EntityFrameworkCore.Contexts
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

        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerComment> AnswerComments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionComment> QuestionComments { get; set; }
        public DbSet<QuestionVote> QuestionVotes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

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