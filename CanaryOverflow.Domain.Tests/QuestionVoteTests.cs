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
            var upvote = QuestionVote.CreateUpvote(null, null);
            upvote.IsFailure.Should().BeTrue();
        }
        
        [Fact]
        public void CreateUpvoteTest()
        {
            var user = new User();
            var result = Question.Create("test title", "test text", user)
                .Tap(q =>
                {
                    QuestionVote.CreateUpvote(q, user)
                        .Tap(v =>
                    {
                        v.Question.Should().BeSameAs(q);
                        v.VotedBy.Should().BeSameAs(user);
                        v.Vote.Should().Be(1);
                    });
                });
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void FailedCreateDownvote()
        {
            var downvote = QuestionVote.CreateDownvote(null, null);
            downvote.IsFailure.Should().BeTrue();
        }
        
        [Fact]
        public void CreateDownvoteTest()
        {
            var user = new User();
            var result = Question.Create("test title", "test text", user)
                .Tap(q =>
                {
                    QuestionVote.CreateDownvote(q, user)
                        .Tap(v =>
                        {
                            v.Question.Should().BeSameAs(q);
                            v.VotedBy.Should().BeSameAs(user);
                            v.Vote.Should().Be(-1);
                        });
                });
            result.IsSuccess.Should().BeTrue();
        }
    }
}