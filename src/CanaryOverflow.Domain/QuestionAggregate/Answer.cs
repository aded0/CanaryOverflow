using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
[JsonConverter(typeof(AnswerJsonConverter))]
public class Answer : Entity<Guid>
{
    public static Answer Create(Guid id, string? text, Guid answeredById, DateTime createdAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifier is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
        if (answeredById == Guid.Empty)
            throw new ArgumentException("Identifier of user is empty.", nameof(answeredById));

        return new Answer(id, text, answeredById, createdAt);
    }

    private readonly HashSet<Comment> _comments;

    private Answer(Guid id, string text, Guid answeredById, DateTime createdAt) : base(id) =>
        (Text, AnsweredById, CreatedAt, _comments) = (text, answeredById, createdAt, new HashSet<Comment>());

    public string Text { get; private set; }
    public Guid AnsweredById { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IEnumerable<Comment> Comments => _comments;

    public void AddComment(string text, Guid commentedById)
    {
        var comment = Comment.Create(Guid.NewGuid(), text, commentedById, DateTime.Now);
        _comments.Add(comment);
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
        Text = text;
    }
}

internal class AnswerJsonConverter : JsonConverter<Answer>
{
    public override Answer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Answer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(nameof(Answer.Id), value.Id);
        writer.WriteString(nameof(Answer.Text), value.Text);
        writer.WriteString(nameof(Answer.AnsweredById), value.AnsweredById);
        writer.WriteString(nameof(Answer.CreatedAt), value.CreatedAt);

        writer.WritePropertyName(nameof(Answer.Comments));
        JsonSerializer.Serialize(writer, value.Comments, options);

        writer.WriteEndObject();
    }
}
