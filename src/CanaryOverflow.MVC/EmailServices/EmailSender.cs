using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CanaryOverflow.MVC.EmailServices;

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;

    public EmailSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return _fluentEmail
            .To(email)
            .Subject(subject)
            .Body(htmlMessage, true)
            .SendAsync();
    }
}
