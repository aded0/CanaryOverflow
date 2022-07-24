﻿using System;
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

internal record ProfileSummaryChanged(string? Summary) : IDomainEvent;

#endregion

public class Profile : AggregateRoot<Guid, Profile>
{
    private const string DisplayNameEmpty = "Display name is empty.";

    public static async Task<Profile> Create(Guid id, string? displayName, DateTime createdAt,
        ICreateAvatar createAvatar)
    {
        if (id == Guid.Empty) throw new ArgumentException("Profile id is empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentNullException(nameof(displayName), DisplayNameEmpty);

        var uploadedAvatarId = await createAvatar.CreateAsync(id, displayName);

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

    public async Task ChangeAvatarAsync(Stream newAvatarData, Guid previousAvatarId, IChangeAvatar changeAvatar)
    {
        var uploadedAvatarId = await changeAvatar.ChangeAsync(newAvatarData, previousAvatarId);

        Append(new AvatarChanged(uploadedAvatarId));
    }

    public void ChangeSummary(string? summary)
    {
        Append(new ProfileSummaryChanged(summary));
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

            case ProfileSummaryChanged profileSummaryChanged:
                Apply(profileSummaryChanged);
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

    private void Apply(ProfileSummaryChanged profileSummaryChanged)
    {
        Summary = profileSummaryChanged.Summary;
    }

    #endregion
}
