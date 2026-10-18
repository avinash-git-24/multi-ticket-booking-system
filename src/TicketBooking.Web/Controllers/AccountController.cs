namespace TicketBooking.Web.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketBooking.Web.Models.DTOs;
using TicketBooking.Web.Services;
using TicketBooking.Web.ViewModels.Account;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, IBookingService bookingService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, error, user) = await _userService.AuthenticateAsync(model.Email, model.Password);
        if (!success || user == null)
        {
            ModelState.AddModelError(string.Empty, error ?? "Invalid login credentials.");
            return View(model);
        }

        await SignInUserAsync(user, model.RememberMe);

        _logger.LogInformation("User {Email} logged in successfully.", user.Email);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (user.Roles.Contains("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new RegisterRequestDto
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            PreferredCityId = model.PreferredCityId
        };

        var (success, error, user) = await _userService.RegisterAsync(dto);
        if (!success || user == null)
        {
            ModelState.AddModelError(string.Empty, error ?? "Registration failed.");
            return View(model);
        }

        await SignInUserAsync(user, rememberMe: false);

        TempData["SuccessMessage"] = "Welcome to ShowSphere! Your account was registered successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> LogoutDirect()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User logged out via GET.");
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var vm = new ProfileViewModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            PreferredCityId = user.PreferredCityId,
            Roles = user.Roles,
            StatusMessage = TempData["SuccessMessage"] as string
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var currentUserId))
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _userService.UpdateProfileAsync(
            currentUserId, model.FullName, model.PhoneNumber, model.PreferredCityId);

        if (updated)
        {
            TempData["SuccessMessage"] = "Your profile details have been saved successfully.";
            return RedirectToAction("Profile");
        }

        ModelState.AddModelError(string.Empty, "Unable to save profile changes. Please try again.");
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return RedirectToAction("Login");
        }

        var allBookings = await _bookingService.GetUserBookingsAsync(userId);

        var viewModel = new TicketBooking.Web.ViewModels.Booking.MyBookingsViewModel
        {
            UpcomingBookings = allBookings
                .Where(b => b.Status == "Confirmed" && b.EventDateTime >= DateTime.UtcNow)
                .OrderBy(b => b.EventDateTime)
                .ToList(),
            PastBookings = allBookings
                .Where(b => b.Status == "Confirmed" && b.EventDateTime < DateTime.UtcNow)
                .OrderByDescending(b => b.EventDateTime)
                .ToList(),
            CancelledBookings = allBookings
                .Where(b => b.Status != "Confirmed")
                .OrderByDescending(b => b.CreatedAtUtc)
                .ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInUserAsync(UserDto user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}
