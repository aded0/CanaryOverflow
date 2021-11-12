using System;
using System.Collections.Generic;
using System.Linq;
using CanaryOverflow.Domain.UserAggregate;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain.QuestionAggregate
{
    public sealed class Question : Entity<Guid>
    {
        public static Result<Question> Create(string title, string text, User askedBy)
        {
            var question = new Question();
            return question.UpdateTitle(title)
                .Bind(q => q.UpdateText(text))
                .Check(_ => Result.SuccessIf(() => askedBy is not null, question, "User is null.")
                    .Tap(q => q.AskedBy = askedBy));
        }

        private readonly QuestionStateMachine _questionStateMachine;

        private Question()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            _questionStateMachine = new QuestionStateMachine(State);
            _comments = new HashSet<QuestionComment>();
            _votes = new List<QuestionVote>();
            _answers = new HashSet<Answer>();
            _tags = new HashSet<string>();
        }

        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public long ViewsCount { get; private set; }
        public string Text { get; private set; }
        public User AskedBy { get; private set; }

        private readonly HashSet<string> _tags;
        public IReadOnlyCollection<string> Tags => _tags;

        private readonly HashSet<QuestionComment> _comments;
        public IReadOnlyCollection<QuestionComment> Comments => _comments;

        private readonly HashSet<Answer> _answers;
        public IReadOnlyCollection<Answer> Answers => _answers;
        public DateTime LastAnswer => Answers.Max(a => a.CreatedAt);
        public Answer Answer { get; private set; }

        private readonly List<QuestionVote> _votes;
        public IReadOnlyList<QuestionVote> Votes => _votes;
        public int Rating => Votes.Sum(v => v.Vote);

        public QuestionState State { get; private set; }

        public Result<Question> SetApproved()
        {
            return Result.SuccessIf(_questionStateMachine.SetApproved(), this, "Can not set as approved.")
                .Tap(() => State = QuestionState.Approved);
        }

        public Result<Question> SetAnswered(long answerId)
        {
            var answer = Answers.FirstOrDefault(a => a.Id == answerId);

            return Result.SuccessIf(answer is not null, this, "Answer not found in answers.")
                .Bind(q => Result.SuccessIf(_questionStateMachine.SetAnswered(), q, "Can not set as answered."))
                .Tap(() => Answer = answer)
                .Tap(() => State = QuestionState.Answered);
        }

        public Result<Question> UpdateTitle(string title)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(title), this, "Title is empty or whitespace.")
                .Tap(() => Title = title);
        }

        public Result<Question> UpdateText(string text)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(text), this, "Text is empty or whitespace.")
                .Tap(() => Text = text);
        }

        public void IncrementViews()
        {
            ViewsCount++;
        }

        public Result<Question> AddTag(string tag)
        {
            return Result.SuccessIf(_tags.Add(tag), this, "Tag does not added.");
        }

        public Result<Question> RemoveTag(string tag)
        {
            return Result.SuccessIf(_tags.Remove(tag), this, "Tag does not removed.");
        }

        public Result<Question> AddComment(string text, User commentedBy)
        {
            return QuestionComment.Create(text, commentedBy).Bind(c =>
                Result.SuccessIf(_comments.Add(c), this, "Comment does not added."));
        }

        public Result<Question> RemoveComment(long commentId)
        {
            var numRemoved = _comments.RemoveWhere(c => c.Id == commentId);
            return Result.SuccessIf(numRemoved > 0, this, "Comment does not removed.");
        }

        public Result<Question> AddAnswer(string text, User answeredBy)
        {
            return Answer.Create(text, answeredBy).Bind(a =>
                Result.SuccessIf(_answers.Add(a), this, "Answer does not added."));
        }

        public Result<Question> RemoveAnswer(long answerId)
        {
            var numRemoved = _answers.RemoveWhere(a => a.Id == answerId);
            return Result.SuccessIf(numRemoved > 0, this, "Answer does not removed.");
        }

        public Result<QuestionVote> Upvote(User user)
        {
            return QuestionVote.CreateUpvote(this, user)
                .Tap(v =>
                {
                    var foundIndex = _votes.FindIndex(qv => qv.VotedBy == user);
                    if (foundIndex != -1)
                    {
                        switch (_votes[foundIndex].Vote)
                        {
                            case -1:
                                _votes[foundIndex].ToggleVote();
                                break;

                            case 1:
                                _votes.RemoveAt(foundIndex);
                                break;
                        }
                    }
                    else
                    {
                        _votes.Add(v);
                    }
                });
        }

        public Result<QuestionVote> Downvote(User user)
        {
            return QuestionVote.CreateDownvote(this, user)
                .Tap(v =>
                {
                    var foundIndex = _votes.FindIndex(qv => qv.VotedBy == user);
                    if (foundIndex != -1)
                    {
                        switch (_votes[foundIndex].Vote)
                        {
                            case -1:
                                _votes.RemoveAt(foundIndex);
                                break;

                            case 1:
                                _votes[foundIndex].ToggleVote();
                                break;
                        }
                    }
                    else
                    {
                        _votes.Add(v);
                    }
                });
        }
    }
}