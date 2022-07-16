using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CanaryOverflow.Service.Mvc.TagHelpers;

[UsedImplicitly]
[HtmlTargetElement("a", Attributes = ControllerAttributeName)]
[HtmlTargetElement("a", Attributes = ActionAttributeName)]
[HtmlTargetElement("a", Attributes = ActiveClassAttributeName)]
public class ActiveAnchorTagHelper : TagHelper
{
    private const string ControllerAttributeName = "asp-controller";
    private const string ActionAttributeName = "asp-action";
    private const string ActiveClassAttributeName = "asp-active-class";

    [HtmlAttributeName(ControllerAttributeName)]
    public string? Controller { get; set; }

    [HtmlAttributeName(ActionAttributeName)]
    public string? Action { get; set; }

    [HtmlAttributeName(ActiveClassAttributeName)]
    public string? ActiveClass { get; set; }

    [ViewContext] [HtmlAttributeNotBound] public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(ActiveClass) || !MatchController() || !MatchAction()) return;
        
        if (output.Attributes.TryGetAttribute("class", out var classAttribute))
        {
            var newValue = string.Join(' ', classAttribute.Value, ActiveClass);
            var attribute = new TagHelperAttribute(classAttribute.Name, newValue, classAttribute.ValueStyle);
            output.Attributes.SetAttribute(attribute);
        }
        else
        {
            output.Attributes.Add("class", ActiveClass);
        }
    }

    private bool MatchController()
    {
        return ViewContext.RouteData.Values.TryGetValue("controller", out var controller) &&
               (string?) controller == Controller;
    }

    private bool MatchAction()
    {
        return ViewContext.RouteData.Values.TryGetValue("action", out var action) && (string?) action == Action;
    }
}
