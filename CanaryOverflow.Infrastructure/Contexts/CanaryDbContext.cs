using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.UserAggregate;
using CanaryOverflow.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CanaryOverflow.Infrastructure.Contexts
{
    public class CanaryDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public CanaryDbContext(DbContextOptions<CanaryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<AnswerComment> AnswerComments { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<QuestionComment> QuestionComments { get; set; } = null!;
        public DbSet<QuestionVote> QuestionVotes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new QuestionVoteEntityTypeConfiguration());
            base.OnModelCreating(builder);
        }
    }
}