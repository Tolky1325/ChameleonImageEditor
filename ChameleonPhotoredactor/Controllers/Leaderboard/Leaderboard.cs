using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

public class LeaderboardController : Controller
{

    [HttpGet]
    public IActionResult Leaderboard()
    {
        return View();
    }
}