using System;
using CanaryOverflow.Domain.QuestionAggregate;
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
            var createdByUserId = Guid.NewGuid();
            var commentedByUserId = Guid.NewGuid();

            var answerResult = Answer.Create("answer text", createdByUserId)
                .Bind(a => a.AddComment("answer comment", commentedByUserId))
                .Tap(a => a.Comments.Count.Should().Be(1));

            answerResult.IsFailure.Should().BeFalse();
        }
    }
}