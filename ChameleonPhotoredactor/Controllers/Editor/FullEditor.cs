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

public class FullEditorController : Controller
{
    [HttpGet]
    public IActionResult FullEditor()
    {
        return View("~/Views/Editor/FullEditor.cshtml");
    }

}