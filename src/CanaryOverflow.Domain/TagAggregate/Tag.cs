using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.Services;
using JetBrains.Annotations;

namespace CanaryOverflow.Domain.TagAggregate;

#region Tag domain events

public record TagCreated(string Name, string Summary, string Description) : IDomainEvent;

public record SummaryChanged(string Text) : IDomainEvent;

public record DescriptionUpdated(string Text) : IDomainEvent;

#endregion

[DebuggerDisplay("{Id}", Name = "{Id}")]
public class Tag : AggregateRoot<string, Tag>
{
    private const string SummaryEmpty = "Summary is empty";
    private const string DescriptionEmpty = "Description is empty";

    public static async Task<Tag> Create(string? name, string? summary, string? description, ITagService tagService)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Tag name is empty");
        if (string.IsNullOrWhiteSpace(summary)) throw new ArgumentNullException(nameof(summary), SummaryEmpty);
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description), DescriptionEmpty);
        var isExists = await tagService.IsExistsAsync(name);
        if (isExists) throw new ArgumentException("Tag already exists", nameof(name));

        return new Tag(name, summary, description);
    }

    [UsedImplicitly]
    private Tag()
    {
    }

    private Tag(string name, string shortDescription, string description)
    {
        Append(new TagCreated(name, shortDescription, description));
    }

    public string Summary { get; private set; } = null!;

    /// <summary>
    /// Long description in markdown.
    /// </summary>
    public string Description { get; private set; } = null!;

    public void UpdateSummary(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text), SummaryEmpty);

        Append(new SummaryChanged(text));
    }

    public void UpdateDescription(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text), DescriptionEmpty);

        Append(new DescriptionUpdated(text));
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case TagCreated tagCreated:
                Apply(tagCreated);
                break;

            case SummaryChanged summaryChanged:
                Apply(summaryChanged);
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
        Id = tagCreated.Name;
        Summary = tagCreated.Summary;
        Description = tagCreated.Description;
    }

    private void Apply(SummaryChanged summaryChanged)
    {
        Summary = summaryChanged.Text;
    }

    private void Apply(DescriptionUpdated descriptionUpdated)
    {
        Description = descriptionUpdated.Text;
    }

    #endregion
}
