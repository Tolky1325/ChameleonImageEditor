using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "User")] 
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

            var userStats = await _context.UserStats
                                          .FirstOrDefaultAsync(s => s.UserId == userId);

            if (userStats != null)
            {
                userStats.importCount += 1; 
            }

            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["Error"] = "Please select a file to upload.";
        }

        return RedirectToAction("Library");
    }
}