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
    private const string DisplayNameEmpty = "Display name is empty.";
    private const string AvatarIdEmpty = "AvatarId is empty.";

    public static async Task<Profile> Create(Guid id, string? displayName, DateTime createdAt, Guid avatarId,
        string? summary, IAssetsService assetsService)
    {
        if (id == Guid.Empty) throw new ArgumentException("Profile id is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), DisplayNameEmpty);
        var exists = await assetsService.IsAvatarExistsAsync(avatarId);
        if (exists is false) throw new ArgumentException(AvatarIdEmpty, nameof(avatarId));

        return new Profile(id, displayName, createdAt, avatarId, summary);
    }

    [UsedImplicitly]
    private Profile()
    {
    }

    private Profile(Guid id, string displayName, DateTime createdAt, Guid avatarId, string? summary)
    {
        Append(new ProfileCreated(id, displayName, createdAt, avatarId, summary));
    }

    public string? DisplayName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid AvatarId { get; private set; }
    public string? Summary { get; private set; }

    public void ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), DisplayNameEmpty);

        Append(new DisplayNameChanged(displayName));
    }

    public async Task ChangeAvatar(Guid avatarId, IAssetsService assetsService)
    {
        var exists = await assetsService.IsAvatarExistsAsync(avatarId);
        if (exists is false) throw new ArgumentException(AvatarIdEmpty, nameof(avatarId));

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
