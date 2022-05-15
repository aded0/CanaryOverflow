using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using MediatR;

namespace CanaryOverflow.Domain.TagAggregate;

public record UpdateTag(string Name, string Summary, string Description) : INotification;

public class UpdateTagHandler : INotificationHandler<UpdateTag>
{
    private readonly IAggregateRepository<string, Tag> _tagRepository;

    public UpdateTagHandler(IAggregateRepository<string, Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task Handle(UpdateTag notification, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.FindAsync(notification.Name, CancellationToken.None);

        if (tag.Summary != notification.Summary)
            tag.UpdateSummary(notification.Summary);
        if (tag.Description != notification.Description)
            tag.UpdateDescription(notification.Description);

        await _tagRepository.SaveAsync(tag, CancellationToken.None);
    }
}
