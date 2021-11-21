using System;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public sealed class QuestionVote
    {
        public static Result<QuestionVote> CreateUpvote(Question question, Guid userId)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => question is null, "Question is null."),
                Result.FailureIf(() => userId == Guid.Empty, "User's identifier is empty."));
            return Result.SuccessIf(isSuccess, new QuestionVote(question, userId, 1), error);
        }

        public static Result<QuestionVote> CreateDownvote(Question question, Guid userId)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => question is null, "Question is null."),
                Result.FailureIf(() => userId == Guid.Empty, "User's identifier is empty."));
            return Result.SuccessIf(isSuccess, new QuestionVote(question, userId, -1), error);
        }

        private QuestionVote()
        {
        }

        private QuestionVote(Question question, Guid userId, sbyte vote)
        {
            Question = question;
            VotedById = userId;
            Vote = vote;
        }

        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; }

        public Guid VotedById { get; private set; }
        public User VotedBy { get; private set; }

        public sbyte Vote { get; private set; }

        public void ToggleVote()
        {
            Vote = (sbyte) (Vote == 1 ? -1 : 1);
        }
    }
}