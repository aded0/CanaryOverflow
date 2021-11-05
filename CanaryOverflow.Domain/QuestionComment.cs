using System;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class QuestionComment : Entity
    {
        public string Text { get; private set; }
        public User CommentedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}