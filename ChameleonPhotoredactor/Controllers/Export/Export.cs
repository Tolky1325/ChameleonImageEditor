using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChameleonPhotoredactor.Data; 
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class ExportController : Controller
{
    private readonly ChameleonDbContext _context;

    public ExportController(ChameleonDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Export(int id) 
    {
        if (id <= 0)
        {
            return RedirectToAction("Index", "Home");
        }

        var imageEdit = await _context.ImageEdits
                            .Include(e => e.Image)
                            .FirstOrDefaultAsync(e => e.ImageEditId == id);

        if (imageEdit == null || imageEdit.Image == null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ImageData = imageEdit.Image.ImageData;

        ViewBag.Exposure = imageEdit.ExposureChange;
        ViewBag.Contrast = imageEdit.ContrastChange;
        ViewBag.ImageEditId = id;

        return View();
    }
}