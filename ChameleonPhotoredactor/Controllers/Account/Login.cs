using Microsoft.AspNetCore.Mvc;

namespace ChameleonPhotoredactor.Controllers.Account
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

    }
}