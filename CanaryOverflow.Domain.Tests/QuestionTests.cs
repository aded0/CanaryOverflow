using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests
{
    public class QuestionTests
    {
        [Fact]
        public void SetAnswerTest()
        {
            var question = Question.Create("test title", "test question", new User());
            var answer = new Answer();
            
            question.Bind(q => q.SetApproved())
                .Bind(q => q.SetAnswered(answer))
                .Tap(q => q.Answer.Should().BeSameAs(answer));
            question.IsFailure.Should().BeFalse();
        }
    }
}