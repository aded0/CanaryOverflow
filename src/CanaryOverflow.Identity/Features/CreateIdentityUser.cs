using CanaryOverflow.Email;
using CanaryOverflow.Identity.Models;
using CSharpFunctionalExtensions;
using FluentEmail.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CanaryOverflow.Identity.Features;

public record CreateIdentityUserCommand
    (string DisplayName, string Email, string Password, string CallbackUri) : IRequest<
        Result<SendResponse, CreateIdentityUserError>>;

public record SendEmailArgs(User User, string CallbackUri, string Email, string DisplayName);

public record CreateIdentityUserError(IReadOnlyCollection<string> Descriptions);

public class
    CreateIdentityUserHandler : IRequestHandler<CreateIdentityUserCommand,
        Result<SendResponse, CreateIdentityUserError>>
{
    private readonly UserManager<User> _userManager;
    private readonly ConfirmationSender _emailSender;

    public CreateIdentityUserHandler(UserManager<User> userManager, ConfirmationSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public Task<Result<SendResponse, CreateIdentityUserError>> Handle(CreateIdentityUserCommand command,
        CancellationToken cancellationToken)
    {
        return CreateUserAsync(command).Bind(SendEmailAsync);
    }

    public async Task<Result<SendEmailArgs, CreateIdentityUserError>> CreateUserAsync(
        CreateIdentityUserCommand command)
    {
        var user = new User(command.DisplayName, command.Email);

        var identityResult = await _userManager.CreateAsync(user, command.Password);
        if (identityResult.Succeeded)
            return new SendEmailArgs(user, command.CallbackUri, command.Email, command.DisplayName);

        var descriptions = identityResult.Errors.Select(e => e.Description).ToList();
        return new CreateIdentityUserError(descriptions);
    }

    public async Task<Result<SendResponse, CreateIdentityUserError>> SendEmailAsync(SendEmailArgs args)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(args.User);
        var queryParams = new Dictionary<string, string>
        {
            {"userId", args.User.Id.ToString()},
            {"code", code}
        };
        var confirmationUri = QueryHelpers.AddQueryString(args.CallbackUri, queryParams);

        return await _emailSender.SendConfirmationEmailAsync(args.Email, args.DisplayName, confirmationUri);
    }
}
