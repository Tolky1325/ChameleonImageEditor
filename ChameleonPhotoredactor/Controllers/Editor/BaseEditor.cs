using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System;
using BCrypt.Net;

public class EditorController : Controller
{
    private readonly ChameleonDbContext _context;

    public EditorController(ChameleonDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public IActionResult BaseEditor()
    {
        
        
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

            string tempUsername = "guest_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            string tempEmail = $"{tempUsername}@temp.com";



            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

            var guestUser = new User(
                userName: tempUsername,
                userDisplayName: "Guest",
                userEmail: tempEmail,
                userPassword: hashedPassword,
                userProfilePic: null,
                userCreationDate: DateTime.UtcNow,
                isTemp: true
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


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, guestUser.userId.ToString()),
                new Claim(ClaimTypes.Name, guestUser.userName),
                new Claim("DisplayName", guestUser.userDisplayName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));


            userId = guestUser.userId;
        }



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
                //^^Links to either the logged-in user or new guest
                imageName: Path.GetFileName(file.FileName),
                imageData: fileData,
                imageType: file.ContentType
            );

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            ViewBag.ImageData = fileData;
            ViewBag.ImageType = file.ContentType;
        }
        else
        {
            TempData["Error"] = "Please select a file to upload.";
        }

        return View();
    }


}