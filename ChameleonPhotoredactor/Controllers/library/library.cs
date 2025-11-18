using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System.Security.Claims;
using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization; 

[Authorize] 
public class LibraryController : Controller
{
    private readonly ChameleonDbContext _context;

    public LibraryController(ChameleonDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Library()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ImportImage(IFormFile file)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (file != null && file.Length > 0)
        {
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

            var image = new Image(
                userId: userId,
                imageName: Path.GetFileName(file.FileName),
                imageData: fileData,
                imageType: file.ContentType
            );

            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["Error"] = "Please select a file to upload.";
        }

        return RedirectToAction("Library");
    }
}