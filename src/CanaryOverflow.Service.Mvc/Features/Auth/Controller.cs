using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanaryOverflow.Service.Mvc.Features.Auth;

[AllowAnonymous]
[AutoValidateAntiforgeryToken]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        ILogger<AuthController> logger,
        SignInManager<IdentityUser<Guid>> signInManager,
        UserManager<IdentityUser<Guid>> userManager,
        IEmailSender emailSender)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public IActionResult Signup(string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            ViewData["ReturnUrl"] = Url.Content("~/");
        }
        else
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        return View("/Features/Auth/Signup.cshtml");
    }

    [HttpPost]
    [ActionName("Signup")]
    public async Task<IActionResult> SignupPost([FromForm] SignupViewModel vm)
    {
        if (!ModelState.IsValid) return View("/Features/Auth/Signup.cshtml", vm);

        var user = new IdentityUser<Guid>();
        await _userManager.SetUserNameAsync(user, vm.Email);
        await _userManager.SetEmailAsync(user, vm.Email);
        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("/Features/Auth/Signup.cshtml", vm);
        }

        _logger.LogInformation("User created a new account with password");

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        var callbackUrl = Url.Action(
            "ConfirmEmail",
            "Auth",
            new {userId, code},
            Request.Scheme);

        await _emailSender.SendEmailAsync(vm.Email, "Confirm your email",
            $"Please confirm your account by <a href='{WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction("RegisterConfirmation", "Auth", new {email = vm.Email});
    }


    public async Task<IActionResult> RegisterConfirmation(string? email)
    {
        if (email is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return NotFound($"Unable to load user with email '{email}'.");
        }

        return View("/Features/Auth/RegisterConfirmation.cshtml");
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

    public IActionResult Login(string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            ViewData["ReturnUrl"] = Url.Content("~/");
        }
        else
        {
            ViewData["ReturnUrl"] = returnUrl;
        }

        return View("/Features/Auth/Login.cshtml");
    }

    [HttpPost]
    [ActionName("Login")]
    public async Task<IActionResult> LoginPost([FromForm] LoginViewModel vm, string returnUrl)
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
            return RedirectToAction("ForgotPasswordConfirmation", "Auth");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "ResetPassword",
            "Auth",
            new {code},
            Request.Scheme);

        await _emailSender.SendEmailAsync(
            vm.Email,
            "Reset Password",
            $"Please reset your password by <a href='{WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction("ForgotPasswordConfirmation", "Auth");
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
            return RedirectToAction("ResetPasswordConfirmation", "Auth");
        }

        var result = await _userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
        if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Auth");

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
            "Auth",
            new {userId, code},
            Request.Scheme);
        await _emailSender.SendEmailAsync(
            vm.Email,
            "Confirm your email",
            $"Please confirm your account by <a href='{WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.");

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
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return LocalRedirect(returnUrl);
    }

    public IActionResult AccessDenied()
    {
        return View("/Features/Auth/AccessDenied.cshtml");
    }
}
