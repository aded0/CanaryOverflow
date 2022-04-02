using System;
using CanaryOverflow.Domain.QuestionAggregate;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.UnitTests;

public class QuestionStateMachineTests
{
    [Fact]
    [Trait("Category", "Question/Transition/Valid")]
    public void Unapproved_to_approved()
    {
        var machine = new QuestionStateMachine(QuestionState.Unapproved);

        machine.SetApproved();

        machine.State.Should().Be(QuestionState.Approved);
    }

    [Theory]
    [InlineData(QuestionState.Approved, QuestionState.Approved)]
    [InlineData(QuestionState.Answered, QuestionState.Answered)]
    [Trait("Category", "Question/Transition/Invalid")]
    public void Other_to_approved(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetApproved();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }

    [Fact]
    [Trait("Category", "Question/Transition/Valid")]
    public void Approved_to_answered()
    {
        var machine = new QuestionStateMachine(QuestionState.Approved);

        machine.SetAnswered();

        machine.State.Should().Be(QuestionState.Answered);
    }


    [Theory]
    [InlineData(QuestionState.Unapproved, QuestionState.Unapproved)]
    [InlineData(QuestionState.Answered, QuestionState.Answered)]
    [Trait("Category", "Question/Transition/Invalid")]
    public void Other_to_answered(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetAnswered();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }

    [Fact]
    [Trait("Category", "Question/Transition/Valid")]
    public void Answered_to_unanswered()
    {
        var machine = new QuestionStateMachine(QuestionState.Answered);

        machine.SetUnanswered();

        machine.State.Should().Be(QuestionState.Approved);
    }

    [Theory]
    [InlineData(QuestionState.Unapproved, QuestionState.Unapproved)]
    [InlineData(QuestionState.Approved, QuestionState.Approved)]
    [Trait("Category", "Question/Transition/Invalid")]
    public void Other_to_unanswered(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetUnanswered();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }
}
