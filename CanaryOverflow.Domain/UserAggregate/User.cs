using System;
using System.Collections.Generic;
using CanaryOverflow.Domain.QuestionAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.UserAggregate
{
    public class User : Entity<Guid>
    {
        private HashSet<QuestionVote> _votes;
        public IReadOnlyCollection<QuestionVote> Votes => _votes;
    }
}