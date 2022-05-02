using System;
using System.Linq;
using System.Threading.Tasks;
using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.UnitTests.Services;
using FluentAssertions;
using Xunit;

namespace CanaryOverflow.Domain.UnitTests;

public class QuestionTests
{
    private const string Title = "test title";
    private const string Text = "test text";
    private const string AnswerText = "test answer";
    private const string CommentText = "test comment";

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

    [Fact]
    [Trait("Category", "Question/Comment/Valid")]
    public void Add_comment_to_question()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddComment(CommentText, Guid.NewGuid());

        question.Comments.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Comment/Invalid")]
    public void Add_invalid_comment_to_question()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        var act = () => question.AddComment(null, Guid.NewGuid());

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Answer/Comment/Valid")]
    public void Add_valid_comment_to_answer()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer(AnswerText, Guid.NewGuid());
        var answer = question.Answers!.First();

        question.AddCommentToAnswer(answer.Id, "test comment", Guid.NewGuid());

        answer.Comments.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Answer/Comment/Invalid")]
    public void Add_comment_with_empty_text_to_answer()
    {
        var question = new Question(Title, Text, Guid.NewGuid());
        question.AddAnswer(AnswerText, Guid.NewGuid());
        var answer = question.Answers!.First();

        var act = () => question.AddCommentToAnswer(answer.Id, "", Guid.NewGuid());

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Question/Answer/Comment/Invalid")]
    public void Add_comment_with_invalid_answer_to_answer()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        var act = () => question.AddCommentToAnswer(Guid.NewGuid(), "test comment", Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Question/Tag/Add/Valid")]
    public async Task Add_tag()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        await question.AddTag(Guid.NewGuid(), new TrueTagService());

        question.Tags.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Tag/Add/Invalid")]
    public void Add_exists_tag()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        var act = async () => await question.AddTag(Guid.NewGuid(), new FalseTagService());

        act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Question/Tag/Remove/Valid")]
    public async Task Remove_tag()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        await question.AddTag(Guid.NewGuid(), new TrueTagService());

        question.Tags.Should().NotBeEmpty();

        var tagId = question.Tags!.First();

        question.RemoveTag(tagId);

        question.Tags.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Question/Tag/Remove/Invalid")]
    public void Remove_unexists_tag()
    {
        var question = new Question(Title, Text, Guid.NewGuid());

        var act = () => question.RemoveTag(Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }
    //TODO: add tests tests for Upvote, Downvote
}
