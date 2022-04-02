using System;
using System.Collections.Generic;
using System.Diagnostics;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

#region Question domain events

public record QuestionCreated
    (Guid Id, string Title, string Text, Guid AskedByUserId, DateTime CreatedAt) : IDomainEvent;

public record TitleUpdated(string Title) : IDomainEvent;

public record TextUpdated(string Text) : IDomainEvent;

public record AnswerAdded(Answer Answer) : IDomainEvent;

public record QuestionCommentAdded(Comment Comment) : IDomainEvent;

public record QuestionApproved : IDomainEvent;

public record QuestionAnswered(Answer Answer) : IDomainEvent;

#endregion

[DebuggerDisplay("{Id}")]
public class Question : AggregateRoot<Guid, Question>
{
    public static Question Create(string title, string text, Guid askedByUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(title), "Title is empty or whitespace.");
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text), "Text is empty or whitespace.");
        if (askedByUserId == Guid.Empty)
            throw new ArgumentException("User's identifier is empty.", nameof(askedByUserId));

        var question = new Question();

        question.Append(new QuestionCreated(Guid.NewGuid(), title, text, askedByUserId, DateTime.Now));

        return question;
    }

    private readonly IQuestionStateMachine _stateMachine;
    private readonly HashSet<Comment> _comments;

    private Question() : this(QuestionState.Unapproved)
    {
    }

    private Question(QuestionState questionState)
    {
        _stateMachine = new QuestionStateMachine(questionState);
        _answers = new HashSet<Answer>();
        _comments = new HashSet<Comment>();
        // _votes = new List<QuestionVote>();
        // _tags = new HashSet<string>();
    }

    public string Title { get; private set; }
    public string Text { get; private set; }
    public Guid AskedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly HashSet<Answer> _answers;
    public IEnumerable<Answer> Answers => _answers;

    public Answer Answer { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments;

    // public long ViewsCount { get; private set; }

    // private readonly HashSet<string> _tags;
    // public IReadOnlyCollection<string> Tags => _tags;

    // private readonly List<QuestionVote> _votes;
    // public IReadOnlyList<QuestionVote> Votes => _votes;
    // public int Rating => Votes.Sum(v => v.Vote);


    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(title), "Title is empty or whitespace.");

        Append(new TitleUpdated(title));
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text), "Text is empty or whitespace.");

        Append(new TextUpdated(text));
    }

    public void SetApproved()
    {
        Append(new QuestionApproved());
    }

    public Answer AddAnswer(string text, Guid answeredById)
    {
        var answer = Answer.Create(Guid.NewGuid(), text, answeredById, DateTime.Now);
        Append(new AnswerAdded(answer));
        return answer;
    }

    public void SetAnswered(Answer answer)
    {
        var notContains = !_answers.Contains(answer);
        if (notContains) throw new NullReferenceException("Answer not found in answers.");

        Append(new QuestionAnswered(answer));
    }

    public void AddComment(string text, Guid commentedByUserId)
    {
        var comment = Comment.Create(Guid.NewGuid(), text, commentedByUserId, DateTime.Now);
        Append(new QuestionCommentAdded(comment));
    }

    // public void IncrementViews()
    // {
    // ViewsCount++;
    // }

    // public Result<Question> AddTag(string tag)
    // {
    //     return Result.SuccessIf(_tags.Add(tag), this, "Tag does not added.");
    // }

    // public Result<Question> RemoveTag(string tag)
    // {
    //     return Result.SuccessIf(_tags.Remove(tag), this, "Tag does not removed.");
    // }

    // public Result<QuestionVote> Upvote(Guid userId)
    // {
    //     return QuestionVote.CreateUpvote(this, userId)
    //         .Tap(v =>
    //         {
    //             var foundIndex = _votes.FindIndex(qv => qv.VotedById == userId);
    //             if (foundIndex != -1)
    //             {
    //                 switch (_votes[foundIndex].Vote)
    //                 {
    //                     case -1:
    //                         _votes[foundIndex].ToggleVote();
    //                         break;
    //
    //                     case 1:
    //                         _votes.RemoveAt(foundIndex);
    //                         break;
    //                 }
    //             }
    //             else
    //             {
    //                 _votes.Add(v);
    //             }
    //         });
    // }

    // public Result<QuestionVote> Downvote(Guid userId)
    // {
    //     return QuestionVote.CreateDownvote(this, userId)
    //         .Tap(v =>
    //         {
    //             var foundIndex = _votes.FindIndex(qv => qv.VotedById == userId);
    //             if (foundIndex != -1)
    //             {
    //                 switch (_votes[foundIndex].Vote)
    //                 {
    //                     case -1:
    //                         _votes.RemoveAt(foundIndex);
    //                         break;
    //
    //                     case 1:
    //                         _votes[foundIndex].ToggleVote();
    //                         break;
    //                 }
    //             }
    //             else
    //             {
    //                 _votes.Add(v);
    //             }
    //         });
    // }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case QuestionCreated questionCreated:
                Apply(questionCreated);
                break;

            case TitleUpdated titleUpdated:
                Apply(titleUpdated);
                break;

            case TextUpdated textUpdated:
                Apply(textUpdated);
                break;

            case AnswerAdded answerAdded:
                Apply(answerAdded);
                break;

            case QuestionCommentAdded questionCommentAdded:
                Apply(questionCommentAdded);
                break;

            case QuestionApproved questionApproved:
                Apply(questionApproved);
                break;

            case QuestionAnswered questionAnswered:
                Apply(questionAnswered);
                break;

            default:
                throw new NotSupportedException($"Question does not support '{@event.GetType().Name}' event.");
        }
    }


    #region Event appliers

    private void Apply(QuestionCreated questionCreated)
    {
        Id = questionCreated.Id;
        Title = questionCreated.Title;
        Text = questionCreated.Text;
        AskedById = questionCreated.AskedByUserId;
        CreatedAt = questionCreated.CreatedAt;
    }

    private void Apply(TitleUpdated titleUpdated)
    {
        Title = titleUpdated.Title;
    }

    private void Apply(TextUpdated textUpdated)
    {
        Text = textUpdated.Text;
    }


    private void Apply(AnswerAdded answerAdded)
    {
        _answers.Add(answerAdded.Answer);
    }

    private void Apply(QuestionCommentAdded questionCommentAdded)
    {
        _comments.Add(questionCommentAdded.Comment);
    }

    private void Apply(QuestionApproved _)
    {
        _stateMachine.SetApproved();
    }

    private void Apply(QuestionAnswered questionAnswered)
    {
        _stateMachine.SetAnswered();
        Answer = questionAnswered.Answer;
    }

    #endregion
}
