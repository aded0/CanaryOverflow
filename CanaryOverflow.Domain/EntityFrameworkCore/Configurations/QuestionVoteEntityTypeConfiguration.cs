using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CanaryOverflow.Domain.EntityFrameworkCore.Configurations
{
    public class QuestionVoteEntityTypeConfiguration : IEntityTypeConfiguration<QuestionVote>
    {
        public void Configure(EntityTypeBuilder<QuestionVote> builder)
        {
            builder.HasKey(v => new {v.QuestionId, v.UserId});
            builder.HasOne(v => v.Question)
                .WithMany(q => q.Votes)
                .HasForeignKey(v => v.QuestionId);
            builder.HasOne(v => v.VotedBy)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId);
        }
    }
}