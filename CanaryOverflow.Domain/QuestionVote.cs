using System;

namespace CanaryOverflow.Domain
{
    public class QuestionVote
    {
        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; }

        public Guid UserId { get; private set; }
        public User VotedBy { get; private set; }
        
        public byte Vote { get; private set; }
    }
}