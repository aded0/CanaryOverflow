using System;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using JetBrains.Annotations;

namespace CanaryOverflow.Domain.ProfileAggregate;

#region Profile domain events

internal record ProfileCreated
    (Guid Id, string DisplayName, DateTime CreatedAt, Guid AvatarId, string? Summary) : IDomainEvent;

internal record DisplayNameChanged(string NewDisplayName) : IDomainEvent;

internal record AvatarChanged(Guid NewAvatarId) : IDomainEvent;

internal record SummaryChanged(string? Summary) : IDomainEvent;

#endregion

public class Profile : AggregateRoot<Guid, Profile>
{
    public static async Task<Profile> Create(string? displayName, Guid avatarId, string? summary,
        IAssetsService assetsService)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), "Display name can not be empty.");

        var exists = await assetsService.IsAvatarExists(avatarId);
        if (exists is false) throw new ArgumentException("AvatarId can not be empty.", nameof(avatarId));

        return new Profile(displayName, avatarId, summary);
    }

    [UsedImplicitly]
    private Profile()
    {
    }

    private Profile(string displayName, Guid avatarId, string? summary)
    {
        Append(new ProfileCreated(Guid.NewGuid(), displayName, DateTime.Now, avatarId, summary));
    }

    public string? DisplayName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid AvatarId { get; private set; }
    public string? Summary { get; private set; }

    public void ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), "Display name can not be empty.");

        Append(new DisplayNameChanged(displayName));
    }

    public async Task ChangeAvatar(Guid avatarId, IAssetsService assetsService)
    {
        var exists = await assetsService.IsAvatarExists(avatarId);
        if (exists is false) throw new ArgumentException("AvatarId can not be empty.", nameof(avatarId));

        Append(new AvatarChanged(avatarId));
    }

    public void ChangeSummary(string? summary)
    {
        Append(new SummaryChanged(summary));
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case ProfileCreated profileCreated:
                Apply(profileCreated);
                break;

            case DisplayNameChanged displayNameChanged:
                Apply(displayNameChanged);
                break;

            case AvatarChanged avatarChanged:
                Apply(avatarChanged);
                break;

            case SummaryChanged summaryChanged:
                Apply(summaryChanged);
                break;
        }
    }


    #region Event appliers

    private void Apply(ProfileCreated profileCreated)
    {
        Id = profileCreated.Id;
        DisplayName = profileCreated.DisplayName;
        CreatedAt = profileCreated.CreatedAt;
        AvatarId = profileCreated.AvatarId;
        Summary = profileCreated.Summary;
    }

    private void Apply(DisplayNameChanged displayNameChanged)
    {
        DisplayName = displayNameChanged.NewDisplayName;
    }

    private void Apply(AvatarChanged avatarChanged)
    {
        AvatarId = avatarChanged.NewAvatarId;
    }

    private void Apply(SummaryChanged summaryChanged)
    {
        Summary = summaryChanged.Summary;
    }

    #endregion
}
