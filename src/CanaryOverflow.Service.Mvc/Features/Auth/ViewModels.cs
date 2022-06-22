using System.ComponentModel.DataAnnotations;

namespace CanaryOverflow.Service.Mvc.Features.Auth;

public class SignupViewModel
{
    [Required(ErrorMessageResourceType = typeof(Resources.Features.Auth.SignupViewModel),
        ErrorMessageResourceName = nameof(Resources.Features.Auth.SignupViewModel.EmailRequired))]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.Features.Auth.SignupViewModel),
        ErrorMessageResourceName = nameof(Resources.Features.Auth.SignupViewModel.EmailInvalid))]
    [Display(Name = nameof(Resources.Features.Auth.SignupViewModel.Email),
        ResourceType = typeof(Resources.Features.Auth.SignupViewModel))]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; init; }

    [Required(ErrorMessageResourceType = typeof(Resources.Features.Auth.SignupViewModel),
        ErrorMessageResourceName = nameof(Resources.Features.Auth.SignupViewModel.PasswordRequired))]
    [Display(Name = nameof(Resources.Features.Auth.SignupViewModel.Password),
        ResourceType = typeof(Resources.Features.Auth.SignupViewModel))]
    [DataType(DataType.Password)]
    public string? Password { get; init; }

    [Required(ErrorMessageResourceType = typeof(Resources.Features.Auth.SignupViewModel),
        ErrorMessageResourceName = nameof(Resources.Features.Auth.SignupViewModel.PasswordRequired))]
    [Compare(nameof(Password), ErrorMessageResourceType = typeof(Resources.Features.Auth.SignupViewModel),
        ErrorMessageResourceName = nameof(Resources.Features.Auth.SignupViewModel.PasswordsMismatch))]
    [Display(Name = nameof(Resources.Features.Auth.SignupViewModel.ConfirmPassword),
        ResourceType = typeof(Resources.Features.Auth.SignupViewModel))]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; init; }
}

public class LoginViewModel
{
    [Required] [EmailAddress] public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
}

public class ConfirmEmailViewModel
{
    public string? StatusMessage { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required] [EmailAddress] public string? Email { get; set; }
}

public class ResetPasswordViewModel
{
    [Required] [EmailAddress] public string? Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    [Required] public string? Code { get; set; }
}

public class ResendEmailConfirmationViewModel
{
    [Required] [EmailAddress] public string? Email { get; set; }
}
