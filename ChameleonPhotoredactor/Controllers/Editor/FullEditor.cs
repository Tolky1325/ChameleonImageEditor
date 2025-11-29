using BCrypt.Net;
using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.Entities;
using ChameleonPhotoredactor.Models.ViewModels.Editor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "User")]
public class FullEditorController : Controller
{
    [HttpGet]
    public IActionResult FullEditor()
    {
        return View("~/Views/Editor/FullEditor.cshtml");
    }

}