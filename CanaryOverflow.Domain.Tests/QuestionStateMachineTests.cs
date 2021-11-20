using CanaryOverflow.Domain.QuestionAggregate;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests
{
    public class QuestionStateMachineTest
    {
        [Theory]
        [InlineData(QuestionState.Unapproved, true)]
        [InlineData(QuestionState.Approved, false)]
        [InlineData(QuestionState.Answered, false)]
        [Trait("Category", "QuestionTransition")]
        public void ToApprovedTest(QuestionState initialState, bool isSuccess)
        {
            var machine = new QuestionStateMachine(initialState);
            var result = machine.SetApproved();
            result.Should().Be(isSuccess);
        }

        [Theory]
        [InlineData(QuestionState.Unapproved, false)]
        [InlineData(QuestionState.Approved, true)]
        [InlineData(QuestionState.Answered, false)]
        [Trait("Category", "QuestionTransition")]
        public void ToAnsweredTest(QuestionState initialState, bool isSuccess)
        {
            var machine = new QuestionStateMachine(initialState);
            var result = machine.SetAnswered();
            result.Should().Be(isSuccess);
        }
        
        [Theory]
        [InlineData(QuestionState.Unapproved, false)]
        [InlineData(QuestionState.Approved, false)]
        [InlineData(QuestionState.Answered, true)]
        [Trait("Category", "QuestionTransition")]
        public void ToCancelAnswerTest(QuestionState initialState, bool isSuccess)
        {
            var machine = new QuestionStateMachine(initialState);
            var result = machine.SetUnanswered();
            result.Should().Be(isSuccess);
        }
    }
}