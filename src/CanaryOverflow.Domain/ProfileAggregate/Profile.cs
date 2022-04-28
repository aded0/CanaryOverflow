using System;
using System.Collections.Generic;
using CanaryOverflow.Common;

namespace CanaryOverflow.Domain.ProfileAggregate;

public class Profile : AggregateRoot<Guid, Profile>
{
    public Profile()
    {
    }

    public string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AvatarId { get; set; }
    public string Summary { get; set; }
    public string Email { get; set; }
    public HashSet<Guid> Answers { get; set; }
    public HashSet<Guid> Questions { get; set; }

    protected override void When(IDomainEvent @event)
    {
    }
}
