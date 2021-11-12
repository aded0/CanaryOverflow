using System.Linq;
using System.Security.AccessControl;
using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.UserAggregate;
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
                .Tap(q => q.State.Should().Be(QuestionState.Approved));
            result.IsFailure.Should().BeFalse();
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

        [Fact]
        [Trait("Category", "Comments")]
        public void AddCommentTest()
        {
            var createdBy = new User();
            var createResult = Question.Create("test title", "test question", createdBy);

            createResult.IsFailure.Should().BeFalse();

            var commentedBy = new User();

            var result = createResult.Bind(q => q.AddComment("test comment", commentedBy))
                .Tap(q => { q.Comments.Count.Should().Be(1); });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Answers")]
        public void AddAnswerTest()
        {
            var createdBy = new User();
            var createResult = Question.Create("test title", "test question", createdBy);
            createResult.IsFailure.Should().BeFalse();

            var result = createResult.Bind(q => q.AddAnswer("my answer", createdBy))
                .Tap(question => question.Answers.Count.Should().Be(1));
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "State")]
        public void SetAnswerTest()
        {
            var createdBy = new User();
            var questionResult = Question.Create("test title", "test question", createdBy);

            questionResult.Bind(q => q.SetApproved())
                .Bind(q => q.AddAnswer("my answer", createdBy))
                .Bind(q => q.SetAnswered(0))
                .Tap(q =>
                {
                    q.Answers.Count.Should().Be(1);
                    q.State.Should().Be(QuestionState.Answered);
                });
            questionResult.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Tags")]
        public void AddTagTwiceTest()
        {
            var createdBy = new User();
            var questionResult = Question.Create("test title", "test question", createdBy);

            var addingResult = questionResult.Bind(q => q.AddTag("sql"))
                .Check(q => Result.SuccessIf(q.Tags.Count == 1, q, "single tag expected"))
                .Bind(q => q.AddTag("sql"))
                .Check(q => Result.SuccessIf(q.Tags.Count == 1, q, "single tag expected"));
            addingResult.IsFailure.Should().BeTrue();
        }
    }
}