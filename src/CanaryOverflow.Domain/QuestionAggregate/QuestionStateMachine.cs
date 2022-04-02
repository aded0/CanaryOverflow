using Stateless;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public class QuestionStateMachine : IQuestionStateMachine
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

        public QuestionState State => _stateMachine.State;

        public void SetApproved()
        {
            _stateMachine.Fire(Trigger.Approve);
        }

        public void SetAnswered()
        {
            _stateMachine.Fire(Trigger.Answer);
        }

        public void SetUnanswered()
        {
            _stateMachine.Fire(Trigger.CancelAnswer);
        }
    }
}
