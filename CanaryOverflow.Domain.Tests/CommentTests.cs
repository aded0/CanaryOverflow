using System;
using CanaryOverflow.Domain.QuestionAggregate;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests;

public class CommentTests
{
    private const string Text = "Test comment";

    [Fact]
    [Trait("Category", "Comment/Create/Valid")]
    public void Create_some_comment()
    {
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment = Comment.Create(commentId, Text, userId, DateTime.Now);

        comment.Text.Should().Be(Text);
        comment.Id.Should().Be(commentId);
        comment.CommentedById.Should().Be(userId);
    }

    [Fact]
    [Trait("Category", "Comment/Create/Invalid")]
    public void Create_comment_with_empty_id()
    {
        var act = () => Comment.Create(Guid.Empty, Text, Guid.NewGuid(), DateTime.Now);

        act.Should().Throw<ArgumentException>().WithMessage("Identifier is empty*");
    }

    [Fact]
    [Trait("Category", "Comment/Create/Invalid")]
    public void Create_comment_with_empty_title()
    {
        var act = () => Comment.Create(Guid.NewGuid(), null, Guid.NewGuid(), DateTime.Now);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Comment/Create/Invalid")]
    public void Create_comment_with_empty_user_id()
    {
        var act = () => Comment.Create(Guid.NewGuid(), Text, Guid.Empty, DateTime.Now);

        act.Should().Throw<ArgumentException>().WithMessage("User's identifier is empty*");
    }
}
