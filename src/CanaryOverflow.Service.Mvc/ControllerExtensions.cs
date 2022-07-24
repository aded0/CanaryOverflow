using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace CanaryOverflow.Service.Mvc;

public static class ControllerExtensions
{
    public static PartialViewResult TurboStream<TModel>(this Controller controller,
        [AspMvcPartialView] string? viewName, TModel? model)
    {
        controller.ViewData.Model = model;

        return new PartialViewResult
        {
            ContentType = "text/vnd.turbo-stream.html",
            StatusCode = StatusCodes.Status200OK,
            TempData = controller.TempData,
            ViewData = controller.ViewData,
            ViewName = viewName
        };
    }
}