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

        private Answer(string text, User createdBy)
        {
            Text = text;
            AnsweredBy = createdBy;
            CreatedAt = DateTime.Now;
        }

        public static Result<Answer> Create(string text, User createdBy)
        {
            var (success, _, error) = Result.Combine(
                Result.SuccessIf(() => !string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.SuccessIf(() => createdBy is not null, "User is null.")
            );

            return Result.SuccessIf(success, new Answer(text, createdBy), error);
        }
    }
}