using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Account; // Переконайтеся, що тут є UpdateProfileViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ChameleonPhotoredactor.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly ChameleonDbContext _context;

        public AccountController(ChameleonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.userName == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.userPassword))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                        new Claim(ClaimTypes.Name, user.userName),
                        new Claim("DisplayName", user.userDisplayName),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, user.userEmail)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Library", "Library");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.userName == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }
                if (await _context.Users.AnyAsync(u => u.userEmail == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(model);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new User(
                    model.Username,
                    model.DisplayName,
                    model.Email,
                    hashedPassword,
                    null,
                    DateTime.UtcNow,
                    false,
                    "User"
                );

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userStats = new UserStats(
                    user.userId,
                    0,
                    0,
                    0
                );
                _context.UserStats.Add(userStats);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users
                .Include(u => u.UserStats) 

                .FirstOrDefaultAsync(u => u.userName == userName);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View("~/Views/Account/Profile.cshtml", user);
        }


        [HttpPost]
        [Route("Account/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var userName = User.Identity.Name;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.userName == userName);

            if (user == null)
            {
                return NotFound(new { message = "User not found in database." });
            }

            if (!string.IsNullOrWhiteSpace(model.NewEmail) && model.NewEmail != user.userEmail)
            {
                if (await _context.Users.AnyAsync(u => u.userEmail == model.NewEmail && u.userId != user.userId))
                {
                    return BadRequest(new { message = "This email is already in use by another account." });
                }
                user.userEmail = model.NewEmail;
            }

            if (!string.IsNullOrWhiteSpace(model.NewDisplayName))
            {
                user.userDisplayName = model.NewDisplayName;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Database error while saving changes." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Name, user.userName),
                new Claim("DisplayName", user.userDisplayName),
                new Claim(ClaimTypes.Email, user.userEmail),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Ok(new
            {
                message = "Profile updated successfully.",
                newDisplayName = user.userDisplayName,
                newEmail = user.userEmail
            });
        }

        [HttpPost]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}