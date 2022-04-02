using System;

namespace CanaryOverflow.Domain.QuestionAggregate;

public interface IQuestionStateMachine
{
    /// <summary>
    /// SetApproved transfer state to approved.
    /// </summary>
    /// <exception cref="InvalidOperationException">This transition not allowed from current state.</exception>
    void SetApproved();

    /// <summary>
    /// SetAnswered transfer state to answered.
    /// </summary>
    /// <exception cref="InvalidOperationException">This transition not allowed from current state.</exception>
    void SetAnswered();

    /// <summary>
    /// SetUnanswered transfer state back to approved.
    /// </summary>
    /// <exception cref="InvalidOperationException">This transition not allowed from current state.</exception>
    void SetUnanswered();
}
