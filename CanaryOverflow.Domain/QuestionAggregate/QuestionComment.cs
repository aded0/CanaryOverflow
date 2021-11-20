using System;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public sealed class QuestionComment : Entity
    {
        public static Result<QuestionComment> Create(string text, Guid userId)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.FailureIf(() => userId == Guid.Empty, "User's identifier is empty."));

            return Result.SuccessIf(isSuccess, new QuestionComment(text, userId), error);
        }

        private QuestionComment()
        {
        }

        private QuestionComment(string text, Guid userId)
        {
            Text = text;
            CommentedById = userId;
            CreatedAt = DateTime.Now;
        }

        public string Text { get; private set; }
        
        public Guid CommentedById { get; private set; }
        private User CommentedBy { get; set; }
        
        public DateTime CreatedAt { get; private set; }
    }
}