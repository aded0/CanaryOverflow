using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.QuestionAggregate;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class QuestionAggregateTests : IClassFixture<AggregateRepositoryProvider<Guid, Question>>
{
    private readonly AggregateRepositoryProvider<Guid, Question> _questionRepositoryProvider;

    public QuestionAggregateTests(AggregateRepositoryProvider<Guid, Question> questionRepositoryProvider)
    {
        _questionRepositoryProvider = questionRepositoryProvider;
    }

    [Fact]
    [Trait("Category", "Question/Create")]
    public async Task Create_aggregate()
    {
        using var scope = _questionRepositoryProvider.Services.CreateScope();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<AggregateRepository<Guid, Question>>();

        var askedById = Guid.NewGuid();
        var question = Question.Create("Title test", "Text test", askedById);
        question.AddAnswer("answer 1 text", Guid.NewGuid());
        await aggregateRepository.SaveAsync(question);
    }
}
