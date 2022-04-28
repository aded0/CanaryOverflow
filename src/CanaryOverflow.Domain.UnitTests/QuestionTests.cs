using System;
using System.Linq;
using CanaryOverflow.Domain.QuestionAggregate;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.UnitTests;

public class QuestionTests
{
    private const string Title = "test title";
    private const string Text = "test text";
    private const string AnswerText = "test answer";

    [Fact]
    [Trait("Category", "Question/Create/Valid")]
    public void Create_question()
    {
        var askedByUserId = Guid.NewGuid();

        var question = new Question(Title, Text, askedByUserId);

        question.Title.Should().Be(Title);
        question.Text.Should().Be(Text);
        question.AskedById.Should().Be(askedByUserId);
    }

    [Theory]
    [InlineData(null, Text)]
    [InlineData(Title, null)]
    [Trait("Category", "Question/Create/Invalid")]
    public void Create_question_with_empty_fields(string? title, string? text)
    {
        var askedByUserId = Guid.NewGuid();

        var act = () => new Question(title, text, askedByUserId);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Create/Invalid")]
    public void Create_question_without_user()
    {
        var act = () => new Question(Title, Text, Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Question/Update/Invalid")]
    public void Update_to_invalid_title()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        var act = () => question.UpdateTitle(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Update/Invalid")]
    public void Update_to_empty_text()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        var act = () => question.UpdateText(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Update/Valid")]
    public void Update_to_some_title()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        question.UpdateTitle("new");

        question.Title.Should().Be("new");
    }

    [Fact]
    [Trait("Category", "Question/Update/Valid")]
    public void Update_to_some_text()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        question.UpdateText("new");

        question.Text.Should().Be("new");
    }

    [Fact]
    [Trait("Category", "Question/Answer")]
    public void Add_answer_to_question()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        const string answer1Text = "answer1";
        var user1Id = Guid.NewGuid();

        question.AddAnswer(answer1Text, user1Id);

        question.Answers.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Answer")]
    public void Add_invalid_answer_to_question()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        var user1Id = Guid.NewGuid();

        var act = () => question.AddAnswer(null, user1Id);

        act.Should().Throw<ArgumentNullException>();
        question.Answers.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Transition/Invalid")]
    public void Change_state_from_unapproved_to_answered()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer("answer1", Guid.NewGuid());

        var answerId = question.Answers!.First().Id;
        var act = () => question.SetAnswered(answerId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
    }


    [Fact]
    [Trait("Category", "Question/Transition/Valid")]
    public void Change_state_from_approved_to_answered()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer("answer1Text", Guid.NewGuid());

        var answerId = question.Answers!.First().Id;
        question.SetApproved();
        question.SetAnswered(answerId);

        question.SelectedAnswerId.Should().Be(answerId);
    }

    [Fact]
    [Trait("Category", "Question/Answer/Update")]
    public void Update_answer_to_empty_text()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer(AnswerText, Guid.NewGuid());

        var answerId = question.Answers!.First().Id;
        var act = () => question.UpdateAnswerText(answerId, null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Answer/Update")]
    public void Update_answer_to_new_text()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer(AnswerText, Guid.NewGuid());

        var answer = question.Answers!.First();

        const string newText = "new text";
        question.UpdateAnswerText(answer.Id, newText);

        answer.Text.Should().Be(newText);
    }
    //TODO: add tests tests for AddComment, AddCommentToAnswer, AddTag, RemoveTag, Upvote, Downvote
}
