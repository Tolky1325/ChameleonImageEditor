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
    public async Task<IActionResult> Library()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdStr, out int userId))
        {
            var userImages = await _context.Images
                                     .Where(i => i.UserId == userId)
                                     .OrderByDescending(i => i.ImageUploadDate) 
                                     .ToListAsync();
            return View(userImages);
        }

        return View(new List<Image>());
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