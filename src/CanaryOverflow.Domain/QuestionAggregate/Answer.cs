using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
[JsonConverter(typeof(AnswerJsonConverter))]
public class Answer : Entity<Guid>
{
    #region JsonConverter

    private class AnswerJsonConverter : JsonConverter<Answer>
    {
        public override Answer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not JsonTokenType.StartObject)
                throw new JsonException();

            var answer = new Answer();

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                    return answer;

                if (reader.TokenType is not JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString();
                reader.Read();

                switch (propName)
                {
                    case nameof(Id):
                        answer.Id = reader.GetGuid();
                        break;
                    case nameof(Text):
                        answer.Text = reader.GetString();
                        break;
                    case nameof(AnsweredById):
                        answer.AnsweredById = reader.GetGuid();
                        break;
                    case nameof(CreatedAt):
                        answer.CreatedAt = reader.GetDateTime();
                        break;
                    case nameof(Comments):
                        answer._comments =
                            JsonSerializer.Deserialize<HashSet<Comment>>(ref reader, options) ?? new HashSet<Comment>();
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Answer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Id), value.Id);
            writer.WriteString(nameof(Text), value.Text);
            writer.WriteString(nameof(AnsweredById), value.AnsweredById);
            writer.WriteString(nameof(CreatedAt), value.CreatedAt);

            writer.WritePropertyName(nameof(Comments));
            JsonSerializer.Serialize(writer, value.Comments, options);

            writer.WriteEndObject();
        }
    }

    #endregion

    private Answer()
    {
    }

    internal Answer(Guid id, string? text, Guid answeredById, DateTime createdAt) : base(id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifier is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
        if (answeredById == Guid.Empty)
            throw new ArgumentException("Identifier of user is empty.", nameof(answeredById));

        Text = text;
        AnsweredById = answeredById;
        CreatedAt = createdAt;
        _comments = new HashSet<Comment>();
    }

    private HashSet<Comment> _comments;

    internal Answer(Answer answer) : base(answer.Id)
    {
        Text = answer.Text;
        AnsweredById = answer.AnsweredById;
        CreatedAt = answer.CreatedAt;
        _comments = answer.Comments.Select(c => c).ToHashSet();
    }

    public string? Text { get; internal set; }
    public Guid AnsweredById { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IEnumerable<Comment> Comments => _comments;

    internal void AddComment(Comment comment)
    {
        _comments.Add(comment);
    }
}
