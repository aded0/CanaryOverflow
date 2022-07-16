using CanaryOverflow.Identity.Features;
using CanaryOverflow.Identity.Models;
using CanaryOverflow.Service.Mvc.Features.Notification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CanaryOverflow.Service.Mvc.Features.Auth;

[AllowAnonymous]
[AutoValidateAntiforgeryToken]
public class AuthnController : Controller
{
    private readonly ILogger<AuthnController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMediator _mediator;

    public AuthnController(ILogger<AuthnController> logger, SignInManager<User> signInManager,
        UserManager<User> userManager, IMediator mediator)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _mediator = mediator;
    }

    public IActionResult Signup(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("/Features/Auth/Signup.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Signup([FromForm] SignupViewModel vm, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("/Features/Auth/Signup.cshtml", vm);
        }

        var callbackUri = Url.Action("ConfirmEmail", "Authn")!;
        var result =
            await _mediator.Send(new CreateIdentityUserCommand(vm.DisplayName, vm.Email, vm.Password, callbackUri));

        if (result.IsFailure)
        {
            foreach (var description in result.Error.Descriptions)
            {
                ModelState.AddModelError("", description);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("/Features/Auth/Signup.cshtml", vm);
        }

        return PartialView("/Features/Notification/_Message.cshtml",
            new MessageViewModel("You're almost done! Please check your email to confirm your account."));
    }

    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        if (userId is null || code is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        var message = result.Succeeded
            ? "Thank you for confirming your email."
            : "Error confirming your email.";
        return View("/Features/Auth/ConfirmEmail.cshtml", new ConfirmEmailViewModel
        {
            StatusMessage = message,
        });
    }

    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("/Features/Auth/Login.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginViewModel vm, string? returnUrl)
    {
        if (!ModelState.IsValid) return View("/Features/Auth/Login.cshtml", vm);

        var result =
            await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View("/Features/Auth/Login.cshtml", vm);
        }

        _logger.LogInformation("User logged in");
        return LocalRedirect(returnUrl);
    }

    public IActionResult ForgotPassword()
    {
        return View("/Features/Auth/ForgotPassword.cshtml");
    }

    [HttpPost]
    [ActionName("ForgotPassword")]
    public async Task<IActionResult> ForgotPasswordPost([FromForm] ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View("/Features/Auth/ForgotPassword.cshtml", vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToAction("ForgotPasswordConfirmation", "Authn");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "ResetPassword",
            "Authn",
            new {code},
            Request.Scheme);

        // await _emailSender.SendConfirmationEmailAsync(
        //     vm.Email,
        //     "Reset Password",
        //     $"Please reset your password by <a href='{HttpUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction("ForgotPasswordConfirmation", "Authn");
    }

    public IActionResult ForgotPasswordConfirmation()
    {
        return View("/Features/Auth/ForgotPasswordConfirmation.cshtml");
    }

    public IActionResult ResetPassword(string? code = null)
    {
        if (code is null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }

        var vm = new ResetPasswordViewModel
        {
            Code = code
        };
        return View("/Features/Auth/ResetPassword.cshtml", vm);
    }

    [HttpPost]
    [ActionName("ResetPassword")]
    public async Task<IActionResult> ResetPasswordPost([FromForm] ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View("/Features/Auth/ResetPassword.cshtml", vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction("ResetPasswordConfirmation", "Authn");
        }

        var result = await _userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
        if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Authn");

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("/Features/Auth/ResetPassword.cshtml", vm);
    }

    public IActionResult ResetPasswordConfirmation()
    {
        return View("/Features/Auth/ResetPasswordConfirmation.cshtml");
    }

    public IActionResult ResendEmailConfirmation()
    {
        return View("/Features/Auth/ResendEmailConfirmation.cshtml");
    }

    [HttpPost]
    [ActionName("ResendEmailConfirmation")]
    public async Task<IActionResult> ResendEmailConfirmationPost([FromForm] ResendEmailConfirmationViewModel vm)
    {
        if (!ModelState.IsValid) return View("/Features/Auth/ResendEmailConfirmation.cshtml", vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null)
        {
            ModelState.AddModelError("", "Verification email sent. Please check your email.");
            return View("/Features/Auth/ResendEmailConfirmation.cshtml", vm);
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action(
            "ConfirmEmail",
            "Authn",
            new {userId, code},
            Request.Scheme);
        // await _emailSender.SendConfirmationEmailAsync(
        //     vm.Email,
        //     "Confirm your email",
        //     $"Please confirm your account by <a href='{HttpUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.");

        ModelState.AddModelError("", "Verification email sent. Please check your email.");
        return View("/Features/Auth/ResendEmailConfirmation.cshtml", vm);
    }

    public IActionResult Logout(string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            ViewData["ReturnUrl"] = Url.Content("~/");
        }
        else
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        return View("/Features/Auth/Logout.cshtml");
    }

    [HttpPost]
    [ActionName("Logout")]
    public async Task<IActionResult> LogoutPost(string returnUrl)
    {
        //     await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return LocalRedirect(returnUrl);
    }

    public IActionResult AccessDenied()
    {
        return View("/Features/Auth/AccessDenied.cshtml");
    }
}
