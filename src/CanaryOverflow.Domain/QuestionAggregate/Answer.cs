using System;
using System.Collections.Generic;
using System.Diagnostics;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
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
