using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using MediatR;

namespace CanaryOverflow.Domain.TagAggregate.Commands;

public record CreateTag(string Name, string Summary, string Description) : INotification;

public class CreateTagHandler : INotificationHandler<CreateTag>
{
    private readonly IAggregateRepository<string, Tag> _tagRepository;
    private readonly ITagService _tagService;

    public CreateTagHandler(IAggregateRepository<string, Tag> tagRepository, ITagService tagService)
    {
        _tagRepository = tagRepository;
        _tagService = tagService;
    }

    public async Task Handle(CreateTag notification, CancellationToken cancellationToken)
    {
        var tag = await Tag.Create(notification.Name, notification.Summary, notification.Description, _tagService);
        await _tagRepository.SaveAsync(tag, CancellationToken.None);
    }
}
