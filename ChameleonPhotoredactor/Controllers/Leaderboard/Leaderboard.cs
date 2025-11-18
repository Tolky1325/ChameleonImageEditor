using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChameleonPhotoredactor.Data;
// ПЕРЕВІРКА: Припускаємо, що це правильний простір імен для ВАШОЇ ViewModel
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
    public async Task<IActionResult> Leaderboard()
    {
        // 1. Створення LINQ запиту для отримання, сортування та проектування даних
        var usersDataQuery = _context.Users
            .Include(u => u.UserStats)
            .Where(u => u.UserStats != null &&
                        (u.UserStats.importCount > 0 || u.UserStats.editCount > 0 || u.UserStats.exportCount > 0))
            .OrderByDescending(u => u.UserStats.importCount + u.UserStats.editCount + u.UserStats.exportCount)

            // 2. Проектуємо в LeaderboardViewModel
            .Select(u => new LeaderboardViewModel // <--- ВИКОРИСТОВУЙТЕ НАЗВУ ВАШОГО VIEWMODEL
            {
                // ВИПРАВЛЕНО: Використовуємо u.userId
                UserId = u.userId,

                // ВИПРАВЛЕНО: Використовуємо u.userDisplayName (або u.userName) замість u.Username
                Username = u.userDisplayName,

                Imports = u.UserStats.importCount,
                Edits = u.UserStats.editCount,
                Exported = u.UserStats.exportCount,
                Rank = 0
            });

        // Виконуємо запит
        var allUsersRanking = await usersDataQuery.ToListAsync();

        // 3. Локальне обчислення рангу
        for (int i = 0; i < allUsersRanking.Count; i++)
        {
            allUsersRanking[i].Rank = i + 1;
        }

        // 4. Обробка "Вашого рейтингу"
        var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(currentUserIdString, out int currentUserId))
        {
            var currentUserRanking = allUsersRanking.FirstOrDefault(u => u.UserId == currentUserId);
            ViewBag.CurrentUserRanking = currentUserRanking;
        }

        // 5. Повертаємо Топ-5
        return View(allUsersRanking.Take(100).ToList());
    }
}