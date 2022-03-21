using Microsoft.AspNetCore.Identity.UI.Services;

namespace CanaryOverflow.MVC.EmailServices;

public static class ServiceCollectionExtensions
{
    public static FluentEmailServicesBuilder AddEmailSender(
        this IServiceCollection collection,
        string defaultFromEmail,
        string defaultFromName = "",
        string host = "localhost",
        int port = 25)
    {
        return collection
            .AddTransient<IEmailSender, EmailSender>()
            .AddFluentEmail(defaultFromEmail, defaultFromName)
            .AddSmtpSender(host, port);
    }
}
