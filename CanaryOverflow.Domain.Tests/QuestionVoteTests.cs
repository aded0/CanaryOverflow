using System;
using CanaryOverflow.Domain.QuestionAggregate;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests
{
    public class QuestionVoteTest
    {
        [Fact]
        public void FailedCreateUpvoteTest()
        {
            var upvote = QuestionVote.CreateUpvote(null, Guid.Empty);
            upvote.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void CreateUpvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var result = Question.Create("test title", "test text", askedByUserId)
                .Tap(q =>
                {
                    QuestionVote.CreateUpvote(q, askedByUserId)
                        .Tap(v =>
                        {
                            v.Question.Should().BeSameAs(q);
                            v.VotedById.Should().Be(askedByUserId);
                            v.Vote.Should().Be(1);
                        });
                });
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void FailedCreateDownvote()
        {
            var downvote = QuestionVote.CreateDownvote(null, Guid.Empty);
            downvote.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void CreateDownvoteTest()
        {
            var askedByUserId = Guid.NewGuid();
            var result = Question.Create("test title", "test text", askedByUserId)
                .Tap(q =>
                {
                    QuestionVote.CreateDownvote(q, askedByUserId)
                        .Tap(v =>
                        {
                            v.Question.Should().BeSameAs(q);
                            v.VotedById.Should().Be(askedByUserId);
                            v.Vote.Should().Be(-1);
                        });
                });
            result.IsSuccess.Should().BeTrue();
        }
    }
}