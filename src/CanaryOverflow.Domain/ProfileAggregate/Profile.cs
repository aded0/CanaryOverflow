using System;
using System.IO;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using JetBrains.Annotations;

namespace CanaryOverflow.Domain.ProfileAggregate;

#region Profile domain events

internal record ProfileCreated(Guid Id, string DisplayName, DateTime CreatedAt, Guid AvatarId) : IDomainEvent;

internal record DisplayNameChanged(string DisplayName) : IDomainEvent;

internal record AvatarChanged(Guid AvatarId) : IDomainEvent;

internal record SummaryChanged(string? Summary) : IDomainEvent;

#endregion

public class Profile : AggregateRoot<Guid, Profile>
{
    private const string DisplayNameEmpty = "Display name is empty.";

    public static async Task<Profile> Create(Guid id, string? displayName, DateTime createdAt,
        IAvatarService avatarService)
    {
        if (id == Guid.Empty) throw new ArgumentException("Profile id is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), DisplayNameEmpty);

        await using var stream = avatarService.Create(id, displayName);
        var uploadedAvatarId = await avatarService.UploadAsync(stream);

        return new Profile(id, displayName, createdAt, uploadedAvatarId);
    }

    [UsedImplicitly]
    private Profile()
    {
    }

    private Profile(Guid id, string displayName, DateTime createdAt, Guid avatarId)
    {
        Append(new ProfileCreated(id, displayName, createdAt, avatarId));
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

    public async Task ChangeAvatarAsync(Stream newAvatarData, Guid previousAvatarId, IAvatarService avatarService)
    {
        var uploadedAvatarId = await avatarService.UploadAsync(newAvatarData);
        await avatarService.DeleteAsync(previousAvatarId);

        Append(new AvatarChanged(uploadedAvatarId));
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
    }

    private void Apply(DisplayNameChanged displayNameChanged)
    {
        DisplayName = displayNameChanged.DisplayName;
    }

    private void Apply(AvatarChanged avatarChanged)
    {
        AvatarId = avatarChanged.AvatarId;
    }

    private void Apply(SummaryChanged summaryChanged)
    {
        Summary = summaryChanged.Summary;
    }

    #endregion
}
