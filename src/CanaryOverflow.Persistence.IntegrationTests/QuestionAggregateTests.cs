using System;
using System.Linq;
using System.Text.Json;
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
    [Trait("Category", "Answer/Create")]
    public void Serialize_answer_entity()
    {
        var answer = Answer.Create(Guid.NewGuid(), "answer text", Guid.NewGuid(), DateTime.Now);
        answer.AddComment("test comment", Guid.NewGuid());
        var answerJsonString = JsonSerializer.Serialize(answer);
    }

    [Fact]
    [Trait("Category", "Question/Create")]
    public async Task Create_question_aggregate()
    {
        using var scope = _questionRepositoryProvider.Services.CreateScope();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<AggregateRepository<Guid, Question>>();

        var askedById = Guid.NewGuid();
        var answeredById = Guid.NewGuid();
        var commentedById = Guid.NewGuid();

        var question = Question.Create("Title test", "Text test", askedById);
        question.AddAnswer("answer 1 text", answeredById);
        question.UpdateText("Awesome question");
        question.UpdateTitle("MY TITLE");
        question.AddAnswer("answer 2 text", answeredById);
        question.AddComment("comment 1", commentedById);
        question.Approve();
        question.AddComment("comment 2", commentedById);
        question.SetAnswered(question.Answers.First());

        await aggregateRepository.SaveAsync(question);

        var restored = await aggregateRepository.FindAsync(question.Id);
    }
}
