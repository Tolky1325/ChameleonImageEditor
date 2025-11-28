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
using System.Linq; // Додано для використання методу .FirstOrDefault() для помилок

namespace ChameleonPhotoredactor.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly ChameleonDbContext _context;

        public AccountController(ChameleonDbContext context)
        {
            _context = context;
        }

        // ===================================
        //           АУТЕНТИФІКАЦІЯ
        // ===================================

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
                        new Claim(ClaimTypes.Name, user.userName), // Використовується для User.Identity.Name
                        new Claim("DisplayName", user.userDisplayName),
                        new Claim(ClaimTypes.Email, user.userEmail) // Додано Email claim
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

        // ===================================
        //             РЕЄСТРАЦІЯ
        // ===================================

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
                    false
                );

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Створення статистики для нового користувача
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

        // ===================================
        //             ПРОФІЛЬ
        // ===================================

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                // Якщо користувач не автентифікований
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

        // ===================================
        //          РЕДАГУВАННЯ ПРОФІЛЮ (AJAX)
        // ===================================

        [HttpPost]
        [Route("Account/UpdateProfile")] // Можна вказати явно, якщо потрібно
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
        {
            // 1. Перевірка автентифікації
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Отримання userName з поточних Claims
            var userName = User.Identity.Name;

            // 2. Отримання користувача з бази даних
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.userName == userName);

            if (user == null)
            {
                return NotFound(new { message = "User not found in database." });
            }

            // 3. Валідація та оновлення Email
            if (!string.IsNullOrWhiteSpace(model.NewEmail) && model.NewEmail != user.userEmail)
            {
                // Перевірка, чи новий email вже зайнятий
                if (await _context.Users.AnyAsync(u => u.userEmail == model.NewEmail && u.userId != user.userId))
                {
                    return BadRequest(new { message = "This email is already in use by another account." });
                }
                user.userEmail = model.NewEmail;
            }

            // 4. Оновлення Display Name
            if (!string.IsNullOrWhiteSpace(model.NewDisplayName))
            {
                user.userDisplayName = model.NewDisplayName;
            }

            // 5. Збереження змін у базі даних
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Database error while saving changes." });
            }

            // 6. Оновлення аутентифікаційної кукі (Claims)
            // Потрібно перегенерувати та переаутентифікувати користувача, 
            // щоб зміни відображалися на сайті одразу (наприклад, у navbar).
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Name, user.userName),
                new Claim("DisplayName", user.userDisplayName),
                new Claim(ClaimTypes.Email, user.userEmail)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // 7. Успішна відповідь
            return Ok(new
            {
                message = "Profile updated successfully.",
                newDisplayName = user.userDisplayName,
                newEmail = user.userEmail
            });
        }

        // ===================================
        //              ВИХІД
        // ===================================

        [HttpPost]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}