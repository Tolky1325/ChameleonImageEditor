using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChameleonPhotoredactor.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using ChameleonPhotoredactor.Models.Entities;
using System;

public class FullEditorController : Controller
{
    private readonly ChameleonDbContext _context;

    public FullEditorController(ChameleonDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> FullEditor(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Library", "Library");
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
        var userId = int.Parse(userIdStr);

        var image = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == id && i.UserId == userId);
        
        if (image == null)
        {
            return RedirectToAction("Library", "Library");
        }

        var imageEdit = await _context.ImageEdits
                                      .Where(e => e.ImageId == id)
                                      .OrderByDescending(e => e.LastEditDate)
                                      .FirstOrDefaultAsync();

        if (imageEdit == null)
        {
            imageEdit = new ImageEdit(id, 0, 0, 0, 0, null);
            _context.ImageEdits.Add(imageEdit);
            await _context.SaveChangesAsync();
        }

        ViewBag.ImageData = image.ImageData;
        ViewBag.ImageType = image.ImageType;
        ViewBag.ImageId = image.ImageId;
        ViewBag.ImageEditId = imageEdit.ImageEditId;

        ViewBag.InitialExposure = imageEdit.ExposureChange;
        ViewBag.InitialContrast = imageEdit.ContrastChange;

        return View("~/Views/Editor/FullEditor.cshtml");
    }
}