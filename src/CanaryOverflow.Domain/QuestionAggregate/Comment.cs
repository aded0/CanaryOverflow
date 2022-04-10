using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
[JsonConverter(typeof(CommentJsonConverter))]
public class Comment : Entity<Guid>
{
    #region JsonConverter

    private class CommentJsonConverter : JsonConverter<Comment>
    {
        public override Comment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not JsonTokenType.StartObject)
                throw new JsonException();

            var comment = new Comment();

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                    return comment;

                if (reader.TokenType is not JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString();
                reader.Read();

                switch (propName)
                {
                    case nameof(Id):
                        comment.Id = reader.GetGuid();
                        break;
                    case nameof(Text):
                        comment.Text = reader.GetString();
                        break;
                    case nameof(CommentedById):
                        comment.CommentedById = reader.GetGuid();
                        break;
                    case nameof(CreatedAt):
                        comment.CreatedAt = reader.GetDateTime();
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Comment value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(Id), value.Id);
            writer.WriteString(nameof(Text), value.Text);
            writer.WriteString(nameof(CommentedById), value.CommentedById);
            writer.WriteString(nameof(CreatedAt), value.CreatedAt);

            writer.WriteEndObject();
        }
    }

    #endregion

    private Comment()
    {
    }

    internal Comment(Guid id, string text, Guid commentedById, DateTime createdAt) : base(id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifier is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(text);
        if (commentedById == Guid.Empty)
            throw new ArgumentException("User's identifier is empty.", nameof(commentedById));

        Text = text;
        CommentedById = commentedById;
        CreatedAt = createdAt;
    }

    public string? Text { get; private set; }
    public Guid CommentedById { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
