using AscarServiceHub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AscarServiceHub.Pages;

public class GirisModel : PageModel
{
    private readonly AuthService _auth;

    public GirisModel(AuthService auth) => _auth = auth;

    public string? ErrorMessage { get; private set; }
    public string? Username { get; private set; }
    public bool RememberMe { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string username, string password, bool rememberMe)
    {
        Username = username;
        RememberMe = rememberMe;

        if (!_auth.ValidateCredentials(username, password))
        {
            ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "Admin"),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        var returnUrl = Request.Query["ReturnUrl"].FirstOrDefault();
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return LocalRedirect("/panel");
    }
}
