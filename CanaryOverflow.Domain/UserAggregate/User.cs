using System;
using System.Collections.Generic;
using CanaryOverflow.Domain.QuestionAggregate;
using Microsoft.AspNetCore.Identity;

namespace CanaryOverflow.Domain.UserAggregate
{
    public class User : IdentityUser<Guid>
    {
        private HashSet<QuestionVote> _votes;
        public IReadOnlyCollection<QuestionVote> Votes => _votes;
    }
}