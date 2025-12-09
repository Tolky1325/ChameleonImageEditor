using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Editor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        if (id <= 0) return RedirectToAction("Library", "Library");

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
        var userId = int.Parse(userIdStr);

        var image = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == id && i.UserId == userId);
        if (image == null) return RedirectToAction("Library", "Library");

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

        ViewBag.InitialSaturation = imageEdit.SaturationChange;

        return View("~/Views/Editor/FullEditor.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> SaveEdits([FromBody] BaseEditorViewModel model)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Json(new { success = false, message = "Unauthorized" });
        var userId = int.Parse(userIdStr);

        var editToUpdate = await _context.ImageEdits
                                         .Include(e => e.Image)
                                         .FirstOrDefaultAsync(e => e.ImageEditId == model.ImageEditId);

        if (editToUpdate == null || editToUpdate.Image.UserId != userId)
        {
            return Json(new { success = false, message = "Error saving." });
        }

        editToUpdate.ExposureChange = model.Exposure;
        editToUpdate.ContrastChange = model.Contrast;

        editToUpdate.SaturationChange = model.Saturation;

        editToUpdate.LastEditDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}