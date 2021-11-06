using System;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class QuestionVote
    {
        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; }

        public Guid UserId { get; private set; }
        public User VotedBy { get; private set; }

        public sbyte Vote { get; private set; }

        private QuestionVote(Question question, User user, sbyte vote)
        {
            Question = question;
            VotedBy = user;
            Vote = vote;
        }

        public static Result<QuestionVote> CreateUpvote(Question question, User user)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => question is null, "Question is null"),
                Result.FailureIf(() => user is null, "User is null"));
            return Result.SuccessIf(isSuccess, new QuestionVote(question, user, 1), error);
        }

        public static Result<QuestionVote> CreateDownvote(Question question, User user)
        {
            var (isSuccess, _, error) = Result.Combine(
                Result.FailureIf(() => question is null, "Question is null"),
                Result.FailureIf(() => user is null, "User is null"));
            return Result.SuccessIf(isSuccess, new QuestionVote(question, user, -1), error);
        }

        public void ToggleVote()
        {
            Vote = (sbyte) (Vote == 1 ? -1 : 1);
        }
    }
}