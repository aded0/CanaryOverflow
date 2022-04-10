using System;
using System.Linq;
using System.Threading.Tasks;
using CanaryOverflow.Domain.QuestionAggregate;
using CanaryOverflow.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class QuestionAggregateTests : IClassFixture<AggregateRepositoryProvider<Guid, Question>>
{
    private readonly AggregateRepositoryProvider<Guid, Question> _aggregateRepositoryProvider;

    public QuestionAggregateTests(AggregateRepositoryProvider<Guid, Question> aggregateRepositoryProvider)
    {
        _aggregateRepositoryProvider = aggregateRepositoryProvider;
    }

    [Fact]
    [Trait("Category", "Question/Create")]
    public async Task Create_question_aggregate()
    {
        using var scope = _aggregateRepositoryProvider.Services.CreateScope();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Guid, Question>>();
        var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();

        var askedById = Guid.NewGuid();
        var answeredById = Guid.NewGuid();
        var commentedById = Guid.NewGuid();
        var answerCommentedById = Guid.NewGuid();

        var question = new Question("Question title", "Question text", askedById);
        question.AddAnswer("answer 1 text", answeredById);
        question.UpdateText("Awesome question");
        question.UpdateTitle("MY TITLE");
        question.AddComment("comment 1", commentedById);
        question.SetApproved();
        question.AddComment("comment 2", commentedById);
        var firstAnswer = question.Answers!.Select(a => a.Id).First();
        question.SetAnswered(firstAnswer);
        question.AddCommentToAnswer(firstAnswer, "thanks you", answerCommentedById);
        question.UpdateAnswerText(firstAnswer, "suck");
        var tagId = Guid.NewGuid();
        await question.AddTag(tagId, tagService);
        question.RemoveTag(tagId);

        await aggregateRepository.SaveAsync(question);

        var questionRestored = await aggregateRepository.FindAsync(question.Id);
    }
}
