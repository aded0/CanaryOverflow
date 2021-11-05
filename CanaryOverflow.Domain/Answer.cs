using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class Answer : Entity
    {
        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public User AnsweredBy { get; private set; }

        public ICollection<AnswerComment> Comments { get; private set; }
    }
}