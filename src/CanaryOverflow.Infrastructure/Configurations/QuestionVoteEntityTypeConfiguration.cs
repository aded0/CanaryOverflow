using CanaryOverflow.Domain.QuestionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CanaryOverflow.Infrastructure.Configurations
{
    public class QuestionVoteEntityTypeConfiguration : IEntityTypeConfiguration<QuestionVote>
    {
        public void Configure(EntityTypeBuilder<QuestionVote> builder)
        {
            builder.HasKey(v => new {v.QuestionId, v.VotedById});
        }
    }
}