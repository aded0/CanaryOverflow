using FluentValidation;

namespace CanaryOverflow.MVC.Features.Auth;

public static class ValidatorsServiceCollectionExtensions
{
    public static IServiceCollection AddAuthValidators(this IServiceCollection services)
    {
        return services.AddScoped<IValidator<SignupViewModel>, SignupViewModelValidator>();
    }
}
