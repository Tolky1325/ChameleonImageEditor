using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

public class EditorController : Controller
{

    [HttpGet]
    public IActionResult BaseEditor()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> BaseEditor(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ViewBag.ImagePath = "/uploads/" + fileName;
        }

        return View();
    }
}