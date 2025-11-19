using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.ViewModels.Leaderboard;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

public class LeaderboardController : Controller
{
    private readonly ChameleonDbContext _context;

    public LeaderboardController(ChameleonDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Leaderboard(string sort = "exported", string direction = "desc")
    {
        // 1. Базовий вибір користувачів
        var usersQuery = _context.Users
            .Include(u => u.UserStats)
            .Where(u => u.UserStats != null &&
                        (u.UserStats.importCount > 0 ||
                         u.UserStats.editCount > 0 ||
                         u.UserStats.exportCount > 0));

        // 2. Сортування з урахуванням напрямку
        usersQuery = sort switch
        {
            "imports" => direction == "asc"
                ? usersQuery.OrderBy(u => u.UserStats.importCount)
                : usersQuery.OrderByDescending(u => u.UserStats.importCount),

            "edits" => direction == "asc"
                ? usersQuery.OrderBy(u => u.UserStats.editCount)
                : usersQuery.OrderByDescending(u => u.UserStats.editCount),

            "exported" => direction == "asc"
                ? usersQuery.OrderBy(u => u.UserStats.exportCount)
                : usersQuery.OrderByDescending(u => u.UserStats.exportCount),

            _ => direction == "asc"
                ? usersQuery.OrderBy(u => u.UserStats.importCount + u.UserStats.editCount + u.UserStats.exportCount)
                : usersQuery.OrderByDescending(u => u.UserStats.importCount + u.UserStats.editCount + u.UserStats.exportCount)
        };

        // 3. Мапінг у ViewModel
        var usersList = await usersQuery
            .Select(u => new LeaderboardViewModel
            {
                UserId = u.userId,
                Username = u.userDisplayName,
                Imports = u.UserStats.importCount,
                Edits = u.UserStats.editCount,
                Exported = u.UserStats.exportCount,
                Rank = 0
            })
            .ToListAsync();

        // 4. Перерахунок Rank
        for (int i = 0; i < usersList.Count; i++)
            usersList[i].Rank = i + 1;

        // 5. Поточний користувач
        var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(currentUserIdString, out int currentUserId))
        {
            ViewBag.CurrentUserRanking =
                usersList.FirstOrDefault(u => u.UserId == currentUserId);
        }

        // 6. Передаємо sort та direction у View
        ViewBag.Sort = sort;
        ViewBag.Direction = direction;

        // 7. Повертаємо топ-25
        return View(usersList.Take(25).ToList());
    }
}
