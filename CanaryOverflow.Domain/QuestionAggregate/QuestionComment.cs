using System;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public class QuestionComment : Entity
    {
        public static Result<QuestionComment> Create(string text, User user)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.FailureIf(() => user is null, "User is null."));
            
            return Result.SuccessIf(isSuccess, new QuestionComment(text, user), error);
        }
        
        private QuestionComment(string text, User user)
        {
            Text = text;
            CommentedBy = user;
            CreatedAt = DateTime.Now;
        }

        public string Text { get; private set; }
        public User CommentedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}