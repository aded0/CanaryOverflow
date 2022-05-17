using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using MediatR;

namespace CanaryOverflow.Domain.ProfileAggregate;

public record UpdateProfile(Guid Id, string DisplayName, Stream? AvatarData, string? Summary) : INotification;

public class UpdateProfileHandler : INotificationHandler<UpdateProfile>
{
    private readonly IAggregateRepository<Guid, Profile> _profileRepository;
    private readonly IChangeAvatar _changeAvatar;

    public UpdateProfileHandler(IAggregateRepository<Guid, Profile> profileRepository, IChangeAvatar changeAvatar)
    {
        _profileRepository = profileRepository;
        _changeAvatar = changeAvatar;
    }

    public async Task Handle(UpdateProfile notification, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.FindAsync(notification.Id, CancellationToken.None);

        if (profile.DisplayName != notification.DisplayName)
            profile.ChangeDisplayName(notification.DisplayName);
        if (profile.Summary != notification.Summary)
            profile.ChangeSummary(notification.Summary);
        if (notification.AvatarData?.Length > 0)
            await profile.ChangeAvatarAsync(notification.AvatarData, profile.AvatarId, _changeAvatar);

        await _profileRepository.SaveAsync(profile, CancellationToken.None);
    }
}
