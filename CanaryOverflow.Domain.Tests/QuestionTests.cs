using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests
{
    public class QuestionTests
    {
        [Fact]
        [Trait("Category", "State")]
        public void SetApprovedTest()
        {
            var createdBy = new User();
            var result = Question.Create("test title", "test question", createdBy)
                .Bind(q => q.SetApproved())
                .Tap(q =>
                {
                    q.State.Should().Be(QuestionState.Approved);
                });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "State")]
        public void SetAnswerTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var answer = new Answer();

            question.Bind(q => q.SetApproved())
                .Bind(q => q.SetAnswered(answer))
                .Tap(q =>
                {
                    q.Answer.Should().BeSameAs(answer);
                    q.State.Should().Be(QuestionState.Answered);
                });
            question.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void UpvoteTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var upvotedBy = new User();

            var result = question
                .Tap(q =>
                {
                    var result = q.Upvote(upvotedBy)
                        .Tap(v => v.Vote.Should().Be(1));

                    result.IsFailure.Should().BeFalse();
                }).Tap(q => { q.Votes.Count.Should().Be(1); });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DoubleUpvoteTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var upvotedBy = new User();

            var result = question.Tap(q =>
            {
                q.Upvote(upvotedBy);
                q.Upvote(upvotedBy);
                q.Votes.Count.Should().Be(0);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DoubleDownvoteTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var upvotedBy = new User();

            var result = question.Tap(q =>
            {
                q.Downvote(upvotedBy);
                q.Downvote(upvotedBy);
                q.Votes.Count.Should().Be(0);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DownvoteToUpvoteTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var upvotedBy = new User();

            var result = question.Tap(q =>
            {
                q.Downvote(upvotedBy);
                var result = q.Upvote(upvotedBy)
                    .Tap(qv => qv.Vote.Should().Be(1));

                result.IsFailure.Should().BeFalse();
                q.Votes.Count.Should().Be(1);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void UpvoteToDownvoteTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var upvotedBy = new User();

            var result = question.Tap(q =>
            {
                q.Upvote(upvotedBy);
                var result = q.Downvote(upvotedBy)
                    .Tap(qv => qv.Vote.Should().Be(-1));

                result.IsFailure.Should().BeFalse();
                q.Votes.Count.Should().Be(1);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void RatingTest()
        {
            var createdBy = new User();
            var question = Question.Create("test title", "test question", createdBy);
            var alice = new User();
            var bob = new User();
            var john = new User();

            var result = question.Tap(q =>
            {
                q.Upvote(alice);
                q.Upvote(bob);
                q.Upvote(john);
            }).Tap(q => q.Rating.Should().Be(3));
            result.IsFailure.Should().BeFalse();
        }
    }
}