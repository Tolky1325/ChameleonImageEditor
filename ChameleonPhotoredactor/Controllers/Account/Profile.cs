using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ChameleonPhotoredactor.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
    }
}