using System;
using System.Collections.Generic;
using System.Diagnostics;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.QuestionAggregate;

[DebuggerDisplay("{Id}")]
public class Answer : Entity<Guid>
{
    public static Answer Create(Guid id, string text, Guid createdByUserId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifier is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Identifier of user is empty.", nameof(createdByUserId));

        return new Answer(id, text, createdByUserId);
    }

    private Answer(Guid id, string text, Guid answeredByUserId)
    {
        Id = id;
        Text = text;
        AnsweredById = answeredByUserId;
        CreatedAt = DateTime.Now;
//      _comments = new HashSet<AnswerComment>();
    }

    public string Text { get; private set; }

    public Guid AnsweredById { get; private set; }

    // private User AnsweredBy { get; set; }
    public DateTime CreatedAt { get; private set; }


//         private readonly HashSet<AnswerComment> _comments;
//         public IReadOnlyCollection<AnswerComment> Comments => _comments;

//         public Result<Answer> AddComment(string text, Guid commentedByUserId)
//         {
//             return AnswerComment.Create(text, commentedByUserId)
//                 .Bind(c => Result.SuccessIf(_comments.Add(c), this, "Comment does not added."));
//         }
//
    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
        Text = text;
    }
}
