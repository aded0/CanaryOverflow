using System;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class AnswerComment : Entity
    {
        public string Text { get; private set; }
        public User CommentedBy { get; private set; }
        public DateTime CreatedBy { get; private set; }
    }
}