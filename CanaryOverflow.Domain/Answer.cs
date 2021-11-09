using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class Answer : Entity
    {
        public static Result<Answer> Create(string text, User createdBy)
        {
            var (success, _, error) = Result.Combine(
                Result.SuccessIf(() => !string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.SuccessIf(() => createdBy is not null, "User is null.")
            );

            return Result.SuccessIf(success, new Answer(text, createdBy), error);
        }
        
        private Answer(string text, User createdBy)
        {
            Text = text;
            AnsweredBy = createdBy;
            CreatedAt = DateTime.Now;
            _comments = new HashSet<AnswerComment>();
        }

        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public User AnsweredBy { get; private set; }

        private readonly HashSet<AnswerComment> _comments;
        public IReadOnlyCollection<AnswerComment> Comments => _comments;

        public Result<Answer> AddComment(AnswerComment comment)
        {
            return Result.SuccessIf(_comments.Add(comment), this, "Comment does not added.");
        }
        
        public Result<Answer> UpdateText(string text)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(text), this, "Text is empty or whitespace.")
                .Tap(() => Text = text);
        }
    }
}