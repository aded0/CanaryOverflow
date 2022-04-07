using System;
using System.Diagnostics;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
public class Comment : Entity<Guid>
{
    public static Comment Create(Guid id, string text, Guid userId, DateTime createdAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifier is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(text);
        if (userId == Guid.Empty) throw new ArgumentException("User's identifier is empty.", nameof(userId));

        return new Comment(id, text, userId, createdAt);
    }

    private Comment(Guid id, string text, Guid commentedById, DateTime createdAt) : base(id)
    {
        Text = text;
        CommentedById = commentedById;
        CreatedAt = createdAt;
    }

    public string Text { get; private set; }
    public Guid CommentedById { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
