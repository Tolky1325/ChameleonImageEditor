using ChameleonPhotoredactor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

    public class FullExportController : Controller
    {
        private readonly ChameleonDbContext _context;

        public FullExportController(ChameleonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FullExport(int id)
        {
            if (id <= 0)
                return RedirectToAction("Library", "Library");

            var imageEdit = await _context.ImageEdits
                                           .Include(e => e.Image)
                                           .FirstOrDefaultAsync(e => e.ImageEditId == id);

            if (imageEdit == null || imageEdit.Image == null)
                return RedirectToAction("Library", "Library");

        if (imageEdit.TimeSpent == 1)
        {
            imageEdit.TimeSpent = 2; 

            var userStats = await _context.UserStats.FirstOrDefaultAsync(u => u.UserId == imageEdit.Image.UserId);
            if (userStats != null)
            {
                userStats.exportCount += 1;
                _context.Update(userStats);
            }

            _context.Update(imageEdit);
            await _context.SaveChangesAsync();
        }

        ViewBag.ImageData = imageEdit.Image.ImageData;
            ViewBag.Exposure = imageEdit.ExposureChange;
            ViewBag.Contrast = imageEdit.ContrastChange;
            ViewBag.Saturation = imageEdit.SaturationChange;
            ViewBag.CropData = imageEdit.CropData;

            ViewBag.ImageEditId = id;
            ViewBag.ImageId = imageEdit.ImageId;


            return View("~/Views/Export/FullExport.cshtml");
        }
    }

