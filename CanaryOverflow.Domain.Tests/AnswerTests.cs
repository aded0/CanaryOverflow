using System;
using CanaryOverflow.Domain.QuestionAggregate;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.Tests;

public class AnswerTests
{
    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_empty_id()
    {
        var act = () => Answer.Create(Guid.Empty, "Test text", Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_invalid_user_id()
    {
        var act = () => Answer.Create(Guid.NewGuid(), "Test text", Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_invalid_text()
    {
        var act = () => Answer.Create(Guid.NewGuid(), "", Guid.NewGuid());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer()
    {
        const string text = "Test";
        var createdByUserId = Guid.NewGuid();

        var answer = Answer.Create(Guid.NewGuid(), text, createdByUserId);

        answer.Text.Should().Be(text);
        answer.AnsweredById.Should().Be(createdByUserId);
    }

    [Fact]
    [Trait("Category", "Answer/Update")]
    public void Update_to_empty_text()
    {
        var answer = Answer.Create(Guid.NewGuid(), "Test", Guid.NewGuid());

        var act = () => answer.UpdateText(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Answer/Update")]
    public void Update_to_new_text()
    {
        const string newText = "new text";
        var answer = Answer.Create(Guid.NewGuid(), "Test", Guid.NewGuid());

        answer.UpdateText(newText);

        answer.Text.Should().Be(newText);
    }
}
