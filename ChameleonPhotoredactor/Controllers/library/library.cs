using Microsoft.AspNetCore.Mvc;

namespace ChameleonPhotoredactor.Controllers.library_mystical
{
    public class LibraryController : Controller
    {
        public IActionResult Library()
        {
            return View();
        }
    }
}