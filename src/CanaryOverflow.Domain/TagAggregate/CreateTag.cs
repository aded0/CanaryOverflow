using System;
using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using MediatR;

namespace CanaryOverflow.Domain.TagAggregate;

public record CreateTag(Guid Id, string Name, string Description) : INotification;

public class CreateTagHandler : INotificationHandler<CreateTag>
{
    private readonly IAggregateRepository<Guid, Tag> _tagRepository;
    private readonly ITagService _tagService;

    public CreateTagHandler(IAggregateRepository<Guid, Tag> tagRepository, ITagService tagService)
    {
        _tagRepository = tagRepository;
        _tagService = tagService;
    }

    public async Task Handle(CreateTag notification, CancellationToken cancellationToken)
    {
        var tag = await Tag.Create(notification.Id, notification.Name, notification.Description, _tagService);
        await _tagRepository.SaveAsync(tag, cancellationToken);
    }
}
