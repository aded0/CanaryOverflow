using FluentValidation;

namespace CanaryOverflow.Service.Mvc.Features.Auth;

public static class ValidatorsServiceCollectionExtensions
{
    public static IServiceCollection AddAuthValidators(this IServiceCollection services)
    {
        return services.AddScoped<IValidator<SignupViewModel>, SignupViewModelValidator>();
    }
}
