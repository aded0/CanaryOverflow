using System;
using System.Collections.Generic;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public sealed class Answer : Entity
    {
        public static Result<Answer> Create(string text, Guid createdByUserId)
        {
            var (success, _, error) = Result.Combine(
                Result.SuccessIf(() => !string.IsNullOrWhiteSpace(text), "Text is null or whitespace."),
                Result.SuccessIf(() => createdByUserId != Guid.Empty, "Identifier of user is empty.")
            );

            return Result.SuccessIf(success, new Answer(text, createdByUserId), error);
        }

        private Answer()
        {
        }

        private Answer(string text, Guid answeredByUserId)
        {
            Text = text;
            AnsweredById = answeredByUserId;
            CreatedAt = DateTime.Now;
            _comments = new HashSet<AnswerComment>();
        }

        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Guid AnsweredById { get; private set; }
        private User AnsweredBy { get; set; }

        private readonly HashSet<AnswerComment> _comments;
        public IReadOnlyCollection<AnswerComment> Comments => _comments;

        public Result<Answer> AddComment(string text, Guid commentedByUserId)
        {
            return AnswerComment.Create(text, commentedByUserId)
                .Bind(c => Result.SuccessIf(_comments.Add(c), this, "Comment does not added."));
        }

        public Result<Answer> UpdateText(string text)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(text), this, "Text is empty or whitespace.")
                .Tap(() => Text = text);
        }
    }
}