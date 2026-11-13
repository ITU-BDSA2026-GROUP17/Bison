// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// NOTE: taken from Microsoft.AspNetCore.Identity.UI

using Bison.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.WebUtilities;

public class RegistrationNoPasswordInputModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
    [DataType(DataType.Text)]
    [Display(Name = "Name")]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
    [DataType(DataType.Text)]
    [Display(Name = "Username")]
    public string Username { get; set; } = default!;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = default!;
}

[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly IEmailSender<User> _emailSender;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly ILogger<ExternalLoginModel> _logger;

    [BindProperty]
    public RegistrationNoPasswordInputModel Input { get; set; } = default!;

    public string? ProviderDisplayName { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public ExternalLoginModel(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IUserStore<User> userStore,
        ILogger<ExternalLoginModel> logger,
        ILookupNormalizer lookupNormalizer,
        IEmailSender<User> emailSender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _lookupNormalizer = lookupNormalizer;
        _logger = logger;
        _emailSender = emailSender;
    }

    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(LoggerEventIds.UserLoggedInByExternalProvider, "User logged in with {LoginProvider} provider.", info.LoginProvider);
            }
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new RegistrationNoPasswordInputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)!
                };
            }
            return Page();
        }
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        // Get the information about the user from the external login provider
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        if (ModelState.IsValid)
        {
            if (await TryConnectEmail(info, returnUrl) is IActionResult actionResult)
            {
                // email was found, and either user was linked or not
                // -> no matter send them where ever the functions wants
                return actionResult;
            }

            var user = CreateUser();

            user.Name = Input.Name;

            await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            if (await TryCreateUser(user, info, returnUrl) is IActionResult actionResult1)
                return actionResult1;
        }

        ProviderDisplayName = info.ProviderDisplayName;
        ReturnUrl = returnUrl;
        return Page();
    }

    private async Task<IActionResult?> TryConnectEmail(ExternalLoginInfo info, string returnUrl)
    {
        _logger.LogTrace("Trying to find email '{email}', {user}", Input.Email, await _emailStore.FindByEmailAsync("test@example.com", CancellationToken.None));
        if (
            await _emailStore.FindByEmailAsync(
                _lookupNormalizer.NormalizeEmail(Input.Email), CancellationToken.None
            ) is User foundUser
        )
        {
            _logger.LogTrace("Found email, test if user owns");
            // if email exists in db, check if they own it according to the third party (we assume the third-party verifies it)
            //
            // this check is done to ensure there exists only one account per email.
            if (
                info.Principal.FindFirstValue(ClaimTypes.Email) is string email &&
                _lookupNormalizer.NormalizeEmail(Input.Email) == _lookupNormalizer.NormalizeEmail(email)
            )
            {
                _logger.LogTrace(message: "User does own e-mail");
                // if they own it, link their account to it.
                var linkedResult = await _userManager.AddLoginAsync(foundUser, info);
                if (linkedResult.Succeeded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation(LoggerEventIds.UserLoggedInByExternalProvider, "User linked their account to {Name} provider.", info.LoginProvider);
                    }
                    return RedirectToRoute("/");
                }

                foreach (var error in linkedResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;

                return Page();
            }
            else
            {
                _logger.LogTrace("User does not own e-mail");
                ModelState.AddModelError(string.Empty, "Unable to create account with this e-mail, please try another.");

                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }
        }

        return null;
    }

    private async Task<IActionResult?> TryCreateUser(User user, ExternalLoginInfo info, string returnUrl)
    {
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation(LoggerEventIds.UserCreatedByExternalProvider, "User created an account using {Name} provider.", info.LoginProvider);
                }

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code },
                    protocol: Request.Scheme)!;

                await _emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

                // If account confirmation is required, we need to show the link if we don't have a real email sender
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("./RegisterConfirmation", new { Input.Email });
                }

                await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return null;
    }

    private User CreateUser()
    {
        try
        {
            return Activator.CreateInstance<User>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
        }
    }

    private IUserEmailStore<User> GetEmailStore()
    {
        return !_userManager.SupportsUserEmail
            ? throw new NotSupportedException("The default UI requires a user store with email support.")
            : (IUserEmailStore<User>)_userStore;
    }
}
