using System;
using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using MediatR;

namespace CanaryOverflow.Domain.ProfileAggregate;

public record CreateProfile(Guid Id, string DisplayName, DateTime CreatedAt) : INotification;

public class CreateProfileHandler : INotificationHandler<CreateProfile>
{
    private readonly IAggregateRepository<Guid, Profile> _profileRepository;
    private readonly IAvatarService _avatarService;

    public CreateProfileHandler(IAggregateRepository<Guid, Profile> profileRepository, IAvatarService avatarService)
    {
        _profileRepository = profileRepository;
        _avatarService = avatarService;
    }

    public async Task Handle(CreateProfile notification, CancellationToken cancellationToken)
    {
        var profile = await Profile.Create(notification.Id, notification.DisplayName, notification.CreatedAt,
            _avatarService);
        await _profileRepository.SaveAsync(profile, CancellationToken.None);
    }
}
