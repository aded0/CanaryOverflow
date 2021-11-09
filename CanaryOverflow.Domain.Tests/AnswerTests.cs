using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests
{
    public class AnswerTests
    {
        [Fact]
        public void AddCommentTest()
        {
            var answeredBy = new User();
            var answerResult = Answer.Create("answer text", answeredBy);
            
            answerResult.Tap(a =>
            {
                var commentedBy = new User();
                var addResult = AnswerComment.Create("answer comment", commentedBy)
                    .Check(a.AddComment);
                addResult.IsFailure.Should().BeFalse();
                a.Comments.Count.Should().Be(1);
            });
            answerResult.IsFailure.Should().BeFalse();
        }
    }
}