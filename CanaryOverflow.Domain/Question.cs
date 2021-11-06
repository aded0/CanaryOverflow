using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace CanaryOverflow.Domain
{
    public class Question : Entity<Guid>
    {
        public static Result<Question> Create(string title, string text, User askedBy)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                _votes = new List<QuestionVote>()
            };
            return question.UpdateTitle(title)
                .Bind(q => q.UpdateText(text))
                .Tap(q => q.AskedBy = askedBy);
        }

        private readonly QuestionStateMachine _questionStateMachine;

        private Question()
        {
            _questionStateMachine = new QuestionStateMachine(State);
        }

        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public long ViewsCount { get; private set; }
        public string Text { get; private set; }
        public User AskedBy { get; private set; }

        private HashSet<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags;

        private HashSet<QuestionComment> _comments;
        public IReadOnlyCollection<QuestionComment> Comments => _comments;

        private HashSet<Answer> _answers;
        public IReadOnlyCollection<Answer> Answers => _answers;
        public DateTime LastAnswer => Answers.Max(a => a.CreatedAt);
        public Answer Answer { get; private set; }

        private List<QuestionVote> _votes;
        public IReadOnlyList<QuestionVote> Votes => _votes;
        public int Rating => Votes.Sum(v => v.Vote);

        public QuestionState State { get; private set; }

        public Result<Question> SetApproved()
        {
            return Result.SuccessIf(_questionStateMachine.SetApproved(), this, "Can not set as approved.")
                .Tap(() => State = QuestionState.Approved);
        }

        public Result<Question> SetAnswered(Answer answer)
        {
            return Result.SuccessIf(_questionStateMachine.SetAnswered(), this, "Can not set as answered.")
                .Tap(() => Answer = answer)
                .Tap(() => State = QuestionState.Answered);
        }

        public Result<Question> UpdateTitle(string title)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(title), this, "Title empty or whitespace.")
                .Tap(() => Title = title);
        }

        public Result<Question> UpdateText(string text)
        {
            return Result.SuccessIf(!string.IsNullOrWhiteSpace(text), this, "Text empty or whitespace.")
                .Tap(() => Text = text);
        }

        public void IncrementViews()
        {
            ViewsCount++;
        }

        public Result<Question> AddTag(Tag tag)
        {
            return Result.SuccessIf(_tags.Add(tag), this, "Tag does not added.");
        }

        public Result<Question> RemoveTag(Tag tag)
        {
            return Result.SuccessIf(_tags.Remove(tag), this, "Tag does not removed.");
        }

        public Result<Question> AddComment(QuestionComment comment)
        {
            return Result.SuccessIf(_comments.Add(comment), this, "Comment does not added.");
        }

        public Result<Question> RemoveComment(QuestionComment comment)
        {
            return Result.SuccessIf(_comments.Remove(comment), this, "Comment does not removed.");
        }

        public Result<Question> AddAnswer(Answer answer)
        {
            return Result.SuccessIf(_answers.Add(answer), this, "Answer does not added.");
        }

        public Result<Question> RemoveAnswer(Answer answer)
        {
            return Result.SuccessIf(_answers.Remove(answer), this, "Answer does not removed.");
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