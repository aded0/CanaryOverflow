using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace CanaryOverflow.Service.Mvc.Email;

public class ConfirmationSender
{
    private readonly IFluentEmail _fluentEmail;

    public ConfirmationSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public Task<SendResponse> SendConfirmationEmailAsync(string email, string subject, string confirmationUri)
    {
        return _fluentEmail
            .To(email)
            .Subject(subject)
            //todo: sync template file path
            .UsingTemplateFromEmbedded("CanaryOverflow.Service.Mvc.Email.ConfirmationEmail.cshtml",
                new ConfirmationEmailViewModel(subject, confirmationUri), typeof(ConfirmationEmailViewModel).Assembly)
            .SendAsync();
    }
}
