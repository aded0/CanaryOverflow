using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace CanaryOverflow.Email;

public class ConfirmationSender
{
    private readonly IFluentEmail _fluentEmail;

    public ConfirmationSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public Task<SendResponse> SendConfirmationEmailAsync(string email, string displayName, string confirmationUri)
    {
        return _fluentEmail
            .To(email)
            .Subject("Account confirmation")
            .UsingTemplateFromEmbedded("CanaryOverflow.Email.Generated.ConfirmationEmail.html",
                new ConfirmationEmailViewModel(displayName, confirmationUri),
                typeof(ConfirmationEmailViewModel).Assembly)
            .SendAsync();
    }
}
