using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using JetBrains.Annotations;

namespace CanaryOverflow.Domain.TagAggregate;

#region Tag domain events

public record TagCreated(Guid Id, string Name, string Description) : IDomainEvent;

public record NameUpdated(string Name) : IDomainEvent;

public record DescriptionUpdated(string Description) : IDomainEvent;

#endregion

[DebuggerDisplay("{Name}", Name = "{Id}")]
public class Tag : AggregateRoot<Guid, Tag>
{
    public static async Task<Tag> Create(Guid id, string? name, string? description, ITagService tagService)
    {
        if (id == Guid.Empty) throw new ArgumentException("Tag id is empty", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Tag name is empty");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description), "Tag description is empty");
        var isExists = await tagService.IsExistsAsync(name);
        if (isExists) throw new ArgumentException($"Name already exists", nameof(name));

        return new Tag(id, name, description);
    }

    [UsedImplicitly]
    private Tag()
    {
    }

    private Tag(Guid id, string name, string description)
    {
        Append(new TagCreated(id, name, description));
    }

    public string? Name { get; private set; }
    public string? Description { get; private set; }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name is null or whitespace");

        Append(new NameUpdated(name));
    }

    public void UpdateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description), "Description is null or whitespace");

        Append(new DescriptionUpdated(description));
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case TagCreated tagCreated:
                Apply(tagCreated);
                break;

            case NameUpdated nameUpdated:
                Apply(nameUpdated);
                break;

            case DescriptionUpdated descriptionUpdated:
                Apply(descriptionUpdated);
                break;

            default:
                throw new NotSupportedException($"Tag does not support '{@event.GetType().Name}' event.");
        }
    }

    #region Event appliers

    private void Apply(TagCreated tagCreated)
    {
        Id = tagCreated.Id;
        Name = tagCreated.Name;
        Description = tagCreated.Description;
    }

    private void Apply(NameUpdated nameUpdated)
    {
        Name = nameUpdated.Name;
    }

    private void Apply(DescriptionUpdated descriptionUpdated)
    {
        Description = descriptionUpdated.Description;
    }

    #endregion
}
