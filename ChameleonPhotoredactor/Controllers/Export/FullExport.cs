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

