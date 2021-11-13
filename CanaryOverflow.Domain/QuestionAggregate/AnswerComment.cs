using System;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public class AnswerComment : Entity
    {
        public static Result<AnswerComment> Create(string text, User createdBy)
        {
            var (success, _, error) = Result.Combine(
                Result.SuccessIf(() => !string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.SuccessIf(() => createdBy is not null, "User is null.")
            );

            return Result.SuccessIf(success, new AnswerComment(text, createdBy), error);
        }

        private AnswerComment()
        {
        }

        private AnswerComment(string text, User createdBy)
        {
            Text = text;
            CommentedBy = createdBy;
            CreatedAt = DateTime.Now;
        }

        public string Text { get; private set; }
        public User CommentedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}