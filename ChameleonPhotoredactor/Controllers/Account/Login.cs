using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ChameleonPhotoredactor.Controllers.Account
{
    public class AccountController : Controller
    {
        private ChameleonDbContext _context;
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
                    //here is gona be logic for redirecting to registered user page
                    //maybe creating user session idk 
                    //for timebeing redirect to homepage
                    return RedirectToAction("Index", "Home");
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
                    //^^ no profile pic on registration
                    DateTime.UtcNow,
                    false           
                    //^^ not a temp user
                );

                
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); 
                //^^saving to db creates id via autoincrement
                //for creating stats needed userid
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
    }
}