using System;
using CanaryOverflow.Domain.QuestionAggregate;
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
            var askedByUserId = Guid.NewGuid();
            var result = Question.Create("test title", "test question", askedByUserId)
                .Bind(q => q.SetApproved())
                .Tap(q => q.State.Should().Be(QuestionState.Approved));
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void UpvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var userId = Guid.NewGuid();

            var result = question
                .Tap(q =>
                {
                    var result = q.Upvote(userId)
                        .Tap(v => v.Vote.Should().Be(1));

                    result.IsFailure.Should().BeFalse();
                }).Tap(q => { q.Votes.Count.Should().Be(1); });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DoubleUpvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var userId = Guid.NewGuid();

            var result = question.Tap(q =>
            {
                q.Upvote(userId);
                q.Upvote(userId);
                q.Votes.Count.Should().Be(0);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DoubleDownvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var userId = Guid.NewGuid();

            var result = question.Tap(q =>
            {
                q.Downvote(userId);
                q.Downvote(userId);
                q.Votes.Count.Should().Be(0);
            });
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Votes")]
        public void DownvoteToUpvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var userId = Guid.NewGuid();

            var result = question.Tap(q =>
            {
                q.Downvote(userId);
                var result = q.Upvote(userId)
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
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var userId = Guid.NewGuid();

            var result = question.Tap(q =>
            {
                q.Upvote(userId);
                var result = q.Downvote(userId)
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
            var askedByUserId = Guid.NewGuid();
            var question = Question.Create("test title", "test question", askedByUserId);
            var alice = Guid.NewGuid();
            var bob = Guid.NewGuid();
            var john = Guid.NewGuid();

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
            var askedByUserId = Guid.NewGuid();
            var createResult = Question.Create("test title", "test question", askedByUserId);

            createResult.IsFailure.Should().BeFalse();

            var commentedByUserId = Guid.NewGuid();

            var result = createResult.Bind(q => q.AddComment("test comment", commentedByUserId))
                .Tap(q => q.Comments.Count.Should().Be(1));
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Answers")]
        public void AddAnswerTest()
        {
            var answeredByUserId = Guid.NewGuid();
            var createResult = Question.Create("test title", "test question", answeredByUserId);
            createResult.IsFailure.Should().BeFalse();

            var result = createResult.Bind(q => q.AddAnswer("my answer", answeredByUserId))
                .Tap(question => question.Answers.Count.Should().Be(1));
            result.IsFailure.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "State")]
        public void SetAnswerTest()
        {
            var answeredByUserId = Guid.NewGuid();
            var questionResult = Question.Create("test title", "test question", answeredByUserId);

            questionResult.Bind(q => q.SetApproved())
                .Bind(q => q.AddAnswer("my answer", answeredByUserId))
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
            var askedByUserId = Guid.NewGuid();
            var questionResult = Question.Create("test title", "test question", askedByUserId);

            var addingResult = questionResult.Bind(q => q.AddTag("sql"))
                .Check(q => Result.SuccessIf(q.Tags.Count == 1, q, "single tag expected"))
                .Bind(q => q.AddTag("sql"))
                .Check(q => Result.SuccessIf(q.Tags.Count == 1, q, "single tag expected"));
            addingResult.IsFailure.Should().BeTrue();
        }
    }
}