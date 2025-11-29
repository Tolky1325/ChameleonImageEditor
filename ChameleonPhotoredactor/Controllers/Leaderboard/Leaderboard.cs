using ChameleonPhotoredactor.Data;
using ChameleonPhotoredactor.Models.ViewModels.Leaderboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "User")]
public class LeaderboardController : Controller
{
    private readonly ChameleonDbContext _context;

    public LeaderboardController(ChameleonDbContext context)
    {
        _context = context;
    }

    private async Task<List<LeaderboardViewModel>> GetRankedUsersAsync(string sort, string direction)
    {
        var usersQuery = _context.Users
            .Include(u => u.UserStats)
            .Where(u => u.UserStats != null &&
                        (u.UserStats.importCount > 0 ||
                         u.UserStats.editCount > 0 ||
                         u.UserStats.exportCount > 0));

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
        };

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

        for (int i = 0; i < usersList.Count; i++)
            usersList[i].Rank = i + 1;

        return usersList;
    }

    [HttpGet]
    public async Task<IActionResult> Leaderboard(string sort = "exported", string direction = "desc")
    {
        var usersList = await GetRankedUsersAsync(sort, direction);

        var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(currentUserIdString, out int currentUserId))
        {
            ViewBag.CurrentUserRanking = usersList.FirstOrDefault(u => u.UserId == currentUserId);
        }

        ViewBag.Sort = sort;
        ViewBag.Direction = direction;

        return View(usersList.Take(25).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query, string sort = "exported", string direction = "desc")
    {
        var allUsers = await GetRankedUsersAsync(sort, direction);

        List<LeaderboardViewModel> filteredUsers;

        if (!string.IsNullOrWhiteSpace(query))
        {
            filteredUsers = allUsers
                .Where(u => u.Username != null &&
                            u.Username.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            filteredUsers = allUsers.Take(25).ToList();
        }

        return Json(filteredUsers);
    }
}