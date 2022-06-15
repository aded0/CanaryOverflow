using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using MediatR;

namespace CanaryOverflow.Domain.QuestionAggregate.Commands;

public record UpdateQuestion(Guid Id, string Title, string Body,IEnumerable<string> Tags) : INotification;

public class UpdateQuestionHandler : INotificationHandler<UpdateQuestion>
{
    private readonly IAggregateRepository<Guid, Question> _questionRepository;

    public UpdateQuestionHandler(IAggregateRepository<Guid, Question> questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task Handle(UpdateQuestion notification, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindAsync(notification.Id, CancellationToken.None);

        if (notification.Title != question.Title)
            question.UpdateTitle(notification.Title);
        if (notification.Body != question.Body)
            question.UpdateBody(notification.Body);

        foreach (var tag in notification.Tags.Except(question.Tags!))
        {
            question.AddTag(tag);
        }

        foreach (var tag in question.Tags!.Except(notification.Tags))
        {
            question.RemoveTag(tag);
        }

        await _questionRepository.SaveAsync(question, CancellationToken.None);
    }
}
