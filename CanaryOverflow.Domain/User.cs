using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class User : Entity<Guid>
    {
        private HashSet<QuestionVote> _votes;
        public IReadOnlyCollection<QuestionVote> Votes => _votes;
    }
}