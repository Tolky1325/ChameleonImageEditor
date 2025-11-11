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

public class ExportController : Controller
{

    [HttpGet]
    public IActionResult Export()
    {   
        return View();
    }
}