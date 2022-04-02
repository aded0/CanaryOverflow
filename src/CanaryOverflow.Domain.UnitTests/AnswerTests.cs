using System;
using CanaryOverflow.Domain.QuestionAggregate;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.UnitTests;

public class AnswerTests
{
    private const string AnswerText = "Test text";

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_empty_id()
    {
        var act = () => Answer.Create(Guid.Empty, AnswerText, Guid.NewGuid(), DateTime.Now);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_invalid_user_id()
    {
        var act = () => Answer.Create(Guid.NewGuid(), AnswerText, Guid.Empty, DateTime.Now);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer_with_invalid_text()
    {
        var act = () => Answer.Create(Guid.NewGuid(), "", Guid.NewGuid(), DateTime.Now);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Answer/Create")]
    public void Create_answer()
    {
        const string text = "Test";
        var createdByUserId = Guid.NewGuid();

        var answer = Answer.Create(Guid.NewGuid(), text, createdByUserId, DateTime.Now);

        answer.Text.Should().Be(text);
        answer.AnsweredById.Should().Be(createdByUserId);
    }

    [Fact]
    [Trait("Category", "Answer/Update")]
    public void Update_to_empty_text()
    {
        var answer = Answer.Create(Guid.NewGuid(), AnswerText, Guid.NewGuid(), DateTime.Now);

        var act = () => answer.UpdateText(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Answer/Update")]
    public void Update_to_new_text()
    {
        const string newText = "new text";
        var answer = Answer.Create(Guid.NewGuid(), AnswerText, Guid.NewGuid(), DateTime.Now);

        answer.UpdateText(newText);

        answer.Text.Should().Be(newText);
    }
}
