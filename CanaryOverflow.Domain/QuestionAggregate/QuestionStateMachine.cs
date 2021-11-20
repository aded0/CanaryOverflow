using System;
using Stateless;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public enum QuestionState
    {
        /// <summary>
        /// Initial state. Does not allowed to show.
        /// </summary>
        Unapproved,

        /// <summary>
        /// Approved by moderator. Allowed to show. Currently no answer.
        /// </summary>
        Approved,

        /// <summary>
        /// Author set question as answered.
        /// </summary>
        Answered
    }

    public class QuestionStateMachine
    {
        private enum Trigger
        {
            /// <summary>
            /// Make review and approve to show.
            /// </summary>
            Approve,

            /// <summary>
            /// Got answer.
            /// </summary>
            Answer,

            /// <summary>
            /// Go back and reject answer. 
            /// </summary>
            CancelAnswer
        }

        private readonly StateMachine<QuestionState, Trigger> _stateMachine;

        public QuestionStateMachine(QuestionState initialState)
        {
            _stateMachine = new StateMachine<QuestionState, Trigger>(initialState);
            _stateMachine.Configure(QuestionState.Unapproved).Permit(Trigger.Approve, QuestionState.Approved);
            _stateMachine.Configure(QuestionState.Approved).Permit(Trigger.Answer, QuestionState.Answered);
            _stateMachine.Configure(QuestionState.Answered).Permit(Trigger.CancelAnswer, QuestionState.Approved);
        }

        /// <summary>
        /// SetApproved transfer state to approved.
        /// </summary>
        /// <returns>True if transfer success, otherwise false.</returns>
        public bool SetApproved()
        {
            try
            {
                _stateMachine.Fire(Trigger.Approve);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// SetAnswered transfer state to answered.
        /// </summary>
        /// <returns>True if transfer success, otherwise false.</returns>
        public bool SetAnswered()
        {
            try
            {
                _stateMachine.Fire(Trigger.Answer);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// SetUnanswered transfer state back to approved.
        /// </summary>
        /// <returns>True if transfer success, otherwise false.</returns>
        public bool SetUnanswered()
        {
            try
            {
                _stateMachine.Fire(Trigger.CancelAnswer);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}