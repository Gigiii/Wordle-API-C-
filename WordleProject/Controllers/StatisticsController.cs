using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WordleProject.Data;
using WordleProject.DTOs.Statistics;
using WordleProject.Models;

namespace WordleProject.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly WordleDbContext _context;

    public StatisticsController(WordleDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var userId = GetUserId();
        var stat = await _context.Statistics.FirstOrDefaultAsync(s => s.UserId == userId);

        if (stat == null)
        {
            stat = new Statistic
            {
                UserId = userId,
                GamesPlayed = 0,
                Wins = 0,
                CurrentStreak = 0,
                MaxStreak = 0,
                TotalPoints = 0
            };

            _context.Statistics.Add(stat);
            await _context.SaveChangesAsync();
        }

        return Ok(new StatisticDTO
        {
            GamesPlayed = stat.GamesPlayed,
            Wins = stat.Wins,
            CurrentStreak = stat.CurrentStreak,
            MaxStreak = stat.MaxStreak,
            TotalPoints = stat.TotalPoints
        });
    }

}
