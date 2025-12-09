using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Editor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

public class EditorController : Controller
{
    private readonly ChameleonDbContext _context;

    public EditorController(ChameleonDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> BaseEditor(int? id)
    {
        if (id.HasValue && id.Value > 0)
        {
            
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                
                return View();
            }
            var userId = int.Parse(userIdStr);

            var imageEdit = await _context.ImageEdits
                                .Include(e => e.Image)
                                .FirstOrDefaultAsync(e => e.ImageEditId == id.Value);

            if (imageEdit != null && imageEdit.Image != null && imageEdit.Image.UserId == userId)
            {

                ViewBag.ImageData = imageEdit.Image.ImageData;
                ViewBag.ImageType = imageEdit.Image.ImageType;
                ViewBag.ImageId = imageEdit.Image.ImageId;
                ViewBag.ImageEditId = imageEdit.ImageEditId;

                ViewBag.InitialExposure = imageEdit.ExposureChange;
                ViewBag.InitialContrast = imageEdit.ContrastChange;
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> BaseEditor(IFormFile file)
    {
        int userId;


        if (User.Identity.IsAuthenticated)
        {

            userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        else
        {

            string tempUsername = "guest__" + Guid.NewGuid().ToString("N").Substring(0, 10);
            string tempEmail = $"{tempUsername}@temp.com";



            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

            var guestUser = new User(
                userName: tempUsername,
                userDisplayName: "Guest",
                userEmail: tempEmail,
                userPassword: hashedPassword,
                userProfilePic: null,
                userCreationDate: DateTime.UtcNow,
                isTemp: true,
                role: "Guest"
            );

            _context.Users.Add(guestUser);
            await _context.SaveChangesAsync();


            var userStats = new UserStats(
                userId: guestUser.userId,
                importCount: 0,
                editCount: 0,
                exportCount: 0
            );
            _context.UserStats.Add(userStats);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, guestUser.userId.ToString()),
                new Claim(ClaimTypes.Name, guestUser.userName),
                new Claim("DisplayName", guestUser.userDisplayName),
                new Claim(ClaimTypes.Role, "Guest")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));


            userId = guestUser.userId;
        }



        if (file != null && file.Length > 0)
        //^^if (file != null && file.Length > 0 && file.ContentType == "image/png")
        //for timebeing there is no limiter to specific data type
        //later on i suppose it would be better to do such limiter
        { 
            
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }


            var image = new Image(
                userId: userId,
                //^^Links to either the new or existing guest
                imageName: Path.GetFileName(file.FileName),
                imageData: fileData,
                imageType: file.ContentType
            );

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var imageEdit = new ImageEdit(
            imageId: image.ImageId,
            exposureChange: 0,
            contrastChange: 0,
            saturationChange: 0,
            timeSpent: 0,
            cropData: null
            );

            _context.ImageEdits.Add(imageEdit);
            await _context.SaveChangesAsync();
            

            ViewBag.ImageData = fileData;
            ViewBag.ImageType = file.ContentType;
            ViewBag.ImageId = image.ImageId;
            ViewBag.ImageEditId = imageEdit.ImageEditId;
        }
        else
        {
            TempData["Error"] = "Please select a file to upload.";
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SaveEdits([FromBody] BaseEditorViewModel model)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, message = "User not authenticated." });
        }
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var editToUpdate = await _context.ImageEdits
        .Include(e => e.Image) 
        //^^Include the Image for the security check
        .FirstOrDefaultAsync(e => e.ImageEditId == model.ImageEditId);

        if (editToUpdate == null)
        {
            return Json(new { success = false, message = "Edit record not found." });
        }
        if (editToUpdate.Image.UserId != userId)
        {
            return Json(new { success = false, message = "Access denied." });
        }

        editToUpdate.ExposureChange = model.Exposure;
        editToUpdate.ContrastChange = model.Contrast;
        editToUpdate.LastEditDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Edits updated successfully." });
    }

}