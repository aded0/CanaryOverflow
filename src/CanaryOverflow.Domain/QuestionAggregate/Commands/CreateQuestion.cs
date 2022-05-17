using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using MediatR;

namespace CanaryOverflow.Domain.QuestionAggregate.Commands;

public record CreateQuestion(Guid Id, string Title, string Body, Guid AskedByUserId, DateTime CreatedAt,
    IEnumerable<string> Tags) : INotification;

public class CreateQuestionHandler : INotificationHandler<CreateQuestion>
{
    private readonly IAggregateRepository<Guid, Question> _questionRepository;

    public CreateQuestionHandler(IAggregateRepository<Guid, Question> questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task Handle(CreateQuestion notification, CancellationToken cancellationToken)
    {
        var question = Question.Create(notification.Id, notification.Title, notification.Body,
            notification.AskedByUserId, notification.CreatedAt);

        foreach (var tag in notification.Tags)
            question.AddTag(tag);

        await _questionRepository.SaveAsync(question, CancellationToken.None);
    }
}
