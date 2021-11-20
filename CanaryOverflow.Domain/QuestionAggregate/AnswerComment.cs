using System;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public sealed class AnswerComment : Entity
    {
        public static Result<AnswerComment> Create(string text, Guid createdByUserId)
        {
            var (success, _, error) = Result.Combine(
                Result.SuccessIf(() => !string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.SuccessIf(() => createdByUserId != Guid.Empty, "Identifier of user is zero.")
            );

            return Result.SuccessIf(success, new AnswerComment(text, createdByUserId), error);
        }

        private AnswerComment()
        {
        }

        private AnswerComment(string text, Guid commentedByUserId)
        {
            Text = text;
            CommentedById = commentedByUserId;
            CreatedAt = DateTime.Now;
        }

        public string Text { get; private set; }

        public Guid CommentedById { get; private set; }
        private User CommentedBy { get; set; }
        
        public DateTime CreatedAt { get; private set; }
    }
}