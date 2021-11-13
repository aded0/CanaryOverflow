using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.UserAggregate;
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
            var commentedBy = new User();

            var answerResult = Answer.Create("answer text", answeredBy)
                .Bind(a => a.AddComment("answer comment", commentedBy))
                .Tap(a => a.Comments.Count.Should().Be(1));

            answerResult.IsFailure.Should().BeFalse();
        }
    }
}