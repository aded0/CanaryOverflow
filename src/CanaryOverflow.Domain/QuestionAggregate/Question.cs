using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;

namespace CanaryOverflow.Domain.QuestionAggregate;

#region Question domain events

internal record QuestionCreated
    (Guid Id, string Title, string Body, Guid AskedByUserId, DateTime CreatedAt) : IDomainEvent;

internal record TitleUpdated(string Title) : IDomainEvent;

internal record TextUpdated(string Text) : IDomainEvent;

internal record QuestionApproved : IDomainEvent;

internal record AnswerAdded(Answer Answer) : IDomainEvent;

internal record CommentAdded(Comment Comment) : IDomainEvent;

internal record QuestionAnswered(Guid AnswerId) : IDomainEvent;

internal record CommentAddedToAnswer(Guid AnswerId, Comment Comment) : IDomainEvent;

internal record AnswerTextUpdated(Guid AnswerId, string Text) : IDomainEvent;

internal record TagAdded(string TagId) : IDomainEvent;

internal record TagRemoved(string TagId) : IDomainEvent;

internal record UpvotedBy(Guid UserId) : IDomainEvent;

internal record DownvotedBy(Guid UserId) : IDomainEvent;

#endregion

[DebuggerDisplay("{Id}")]
[JsonConverter(typeof(QuestionJsonConverter))]
public class Question : AggregateRoot<Guid, Question>
{
    #region JsonConverter

    private class QuestionJsonConverter : JsonConverter<Question>
    {
        public override Question Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not JsonTokenType.StartObject)
                throw new JsonException();

            var question = new Question();

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                    return question;

                if (reader.TokenType is not JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString();
                reader.Read();

                switch (propName)
                {
                    case nameof(Id):
                        question.Id = reader.GetGuid();
                        break;

                    case nameof(Title):
                        question.Title = reader.GetString();
                        break;

                    case nameof(Body):
                        question.Body = reader.GetString();
                        break;

                    case nameof(AskedById):
                        question.AskedById = reader.GetGuid();
                        break;

                    case nameof(CreatedAt):
                        question.CreatedAt = reader.GetDateTime();
                        break;

                    case nameof(SelectedAnswerId):
                        question.SelectedAnswerId = reader.GetGuid();
                        break;

                    case nameof(Answers):
                        question._answers = JsonSerializer.Deserialize<HashSet<Answer>>(ref reader, options);
                        break;

                    case nameof(Comments):
                        question._comments = JsonSerializer.Deserialize<HashSet<Comment>>(ref reader, options);
                        break;

                    case nameof(Tags):
                        question._tags = JsonSerializer.Deserialize<HashSet<string>>(ref reader, options);
                        break;

                    case nameof(Rating):
                        question._rating = JsonSerializer.Deserialize<Dictionary<Guid, int>>(ref reader, options);
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Question value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Id), value.Id);
            writer.WriteString(nameof(Title), value.Title);
            writer.WriteString(nameof(Body), value.Body);
            writer.WriteString(nameof(AskedById), value.AskedById);
            writer.WriteString(nameof(CreatedAt), value.CreatedAt);
            writer.WriteString(nameof(SelectedAnswerId), value.SelectedAnswerId);

            writer.WritePropertyName(nameof(Answers));
            JsonSerializer.Serialize(writer, value.Answers, options);

            writer.WritePropertyName(nameof(Comments));
            JsonSerializer.Serialize(writer, value.Comments, options);

            writer.WritePropertyName(nameof(Tags));
            JsonSerializer.Serialize(writer, value._tags, options);

            writer.WritePropertyName(nameof(Rating));
            JsonSerializer.Serialize(writer, value._rating, options);

            writer.WriteEndObject();
        }
    }

    #endregion

    private readonly IQuestionStateMachine _stateMachine;
    private HashSet<Answer>? _answers;
    private HashSet<Comment>? _comments;
    private HashSet<string>? _tags;
    private Dictionary<Guid, int>? _rating;

    public static Question Create(Guid id, string? title, string? body, Guid askedByUserId, DateTime createdAt)
    {
        if (Guid.Empty == id) throw new ArgumentException("Id is empty", nameof(id));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(title), "Title is empty or whitespace.");
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentNullException(nameof(body), "Text is empty or whitespace.");
        if (askedByUserId == Guid.Empty)
            throw new ArgumentException("User's identifier is empty.", nameof(askedByUserId));

        var question = new Question();

        question.Append(new QuestionCreated(id, title, body, askedByUserId, createdAt));

        return question;
    }

    private Question() : this(QuestionState.Unapproved)
    {
    }

    private Question(QuestionState questionState)
    {
        _stateMachine = new QuestionStateMachine(questionState);
        _answers = new HashSet<Answer>();
        _comments = new HashSet<Comment>();
        _tags = new HashSet<string>();
        _rating = new Dictionary<Guid, int>();
    }

    public string? Title { get; private set; }

    /// <summary>
    /// Content formatted in markdown.
    /// </summary>
    public string? Body { get; private set; }

    public Guid AskedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Guid SelectedAnswerId { get; private set; }
    public IEnumerable<Answer>? Answers => _answers;
    public IEnumerable<Comment>? Comments => _comments;
    public IEnumerable<string>? Tags => _tags;
    public IReadOnlyDictionary<Guid, int>? Rating => _rating;

    public void UpdateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException(nameof(title), "Title is empty or whitespace.");

        Append(new TitleUpdated(title));
    }

    public void UpdateText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text), "Text is empty or whitespace.");

        Append(new TextUpdated(text));
    }

    public void SetApproved()
    {
        Append(new QuestionApproved());
    }

    public void AddAnswer(string? text, Guid answeredById)
    {
        var answer = new Answer(Guid.NewGuid(), text, answeredById, DateTime.Now);

        Append(new AnswerAdded(answer));
    }

    public void SetAnswered(Guid answerId)
    {
        var containsAnswer = _answers?.Select(a => a.Id).Contains(answerId);
        if (containsAnswer is not true) throw new NullReferenceException("Answer not found in answers.");

        Append(new QuestionAnswered(answerId));
    }

    public void AddComment(string? text, Guid commentedByUserId)
    {
        var comment = new Comment(Guid.NewGuid(), text, commentedByUserId, DateTime.Now);

        Append(new CommentAdded(comment));
    }

    public void AddCommentToAnswer(Guid answerId, string text, Guid commentedById)
    {
        var containsAnswer = _answers?.Select(a => a.Id).Contains(answerId);
        if (containsAnswer is not true) throw new ArgumentException("Answer not found in answers", nameof(answerId));

        var comment = new Comment(Guid.NewGuid(), text, commentedById, DateTime.Now);

        Append(new CommentAddedToAnswer(answerId, comment));
    }

    public void UpdateAnswerText(Guid answerId, string? text)
    {
        var containsAnswer = _answers?.Select(a => a.Id).Contains(answerId);
        if (containsAnswer is not true) throw new ArgumentException("Answer not found in answers", nameof(answerId));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));

        Append(new AnswerTextUpdated(answerId, text));
    }

    public void AddTag(string tagName)
    {
        Append(new TagAdded(tagName));
    }

    public void RemoveTag(string tagName)
    {
        if (_tags?.Contains(tagName) is not true)
            throw new ArgumentException("Tag was not added to question", nameof(tagName));

        Append(new TagRemoved(tagName));
    }

    public async Task Upvote(Guid userId, IProfileService profileService)
    {
        var exists = await profileService.IsExistsAsync(userId);
        if (!exists) throw new ArgumentException("User does not exists", nameof(userId));

        Append(new UpvotedBy(userId));
    }

    public async Task Downvote(Guid userId, IProfileService profileService)
    {
        var exists = await profileService.IsExistsAsync(userId);
        if (!exists) throw new ArgumentException("User does not exists", nameof(userId));

        Append(new DownvotedBy(userId));
    }

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

            case CommentAdded questionCommentAdded:
                Apply(questionCommentAdded);
                break;

            case QuestionApproved questionApproved:
                Apply(questionApproved);
                break;

            case QuestionAnswered questionAnswered:
                Apply(questionAnswered);
                break;

            case CommentAddedToAnswer commentAddedToAnswer:
                Apply(commentAddedToAnswer);
                break;

            case AnswerTextUpdated answerTextUpdated:
                Apply(answerTextUpdated);
                break;

            case TagAdded tagAdded:
                Apply(tagAdded);
                break;

            case TagRemoved tagRemoved:
                Apply(tagRemoved);
                break;

            case UpvotedBy upvotedBy:
                Apply(upvotedBy);
                break;

            case DownvotedBy downvotedBy:
                Apply(downvotedBy);
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
        Body = questionCreated.Body;
        AskedById = questionCreated.AskedByUserId;
        CreatedAt = questionCreated.CreatedAt;
    }

    private void Apply(TitleUpdated titleUpdated)
    {
        Title = titleUpdated.Title;
    }

    private void Apply(TextUpdated textUpdated)
    {
        Body = textUpdated.Text;
    }

    private void Apply(AnswerAdded answerAdded)
    {
        _answers!.Add(new Answer(answerAdded.Answer));
    }

    private void Apply(CommentAdded commentAdded)
    {
        _comments!.Add(commentAdded.Comment);
    }

    // ReSharper disable once UnusedParameter.Local
    private void Apply(QuestionApproved _)
    {
        _stateMachine.SetApproved();
    }

    private void Apply(QuestionAnswered questionAnswered)
    {
        _stateMachine.SetAnswered();
        SelectedAnswerId = questionAnswered.AnswerId;
    }

    private void Apply(CommentAddedToAnswer commentAddedToAnswer)
    {
        var answer = _answers!.Single(a => a.Id == commentAddedToAnswer.AnswerId);
        answer.AddComment(commentAddedToAnswer.Comment);
    }

    private void Apply(AnswerTextUpdated answerTextUpdated)
    {
        var answer = _answers!.Single(a => a.Id == answerTextUpdated.AnswerId);
        answer.Text = answerTextUpdated.Text;
    }

    private void Apply(TagAdded tagAdded)
    {
        _tags!.Add(tagAdded.TagId);
    }

    private void Apply(TagRemoved tagRemoved)
    {
        _tags!.Remove(tagRemoved.TagId);
    }

    private void Apply(UpvotedBy upvotedBy)
    {
        if (_rating!.TryGetValue(upvotedBy.UserId, out var userRating))
        {
            if (userRating == 1)
            {
                _rating.Remove(upvotedBy.UserId);
                return;
            }
        }

        _rating[upvotedBy.UserId] = 1;
    }

    private void Apply(DownvotedBy downvotedBy)
    {
        if (_rating!.TryGetValue(downvotedBy.UserId, out var userRating))
        {
            if (userRating == -1)
            {
                _rating.Remove(downvotedBy.UserId);
                return;
            }
        }

        _rating[downvotedBy.UserId] = -1;
    }

    #endregion
}
