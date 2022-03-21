using FluentValidation;

namespace CanaryOverflow.MVC.Features.Auth;

public class SignupViewModelValidator : AbstractValidator<SignupViewModel>
{
    public SignupViewModelValidator()
    {
        RuleFor(vm => vm.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(vm => GetPasswordLength(vm))
            .InclusiveBetween(6, 50)
            .OverridePropertyName(vm => vm.Password);

        RuleFor(vm => vm.ConfirmPassword)
            .Must((vm, confirm) => vm.Password == confirm);
    }

    private static int GetPasswordLength(SignupViewModel vm) => vm.Password?.Length ?? 0;
}
