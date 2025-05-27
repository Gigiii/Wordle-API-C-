using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WordleProject.Data;
using WordleProject.Models;
using WordleProject.Services;
using WordleProject.DTOs.Games;
using WordleProject.DTOs.Guesses;

namespace WordleProject.Controllers;

[ApiController]
[Route("api/games")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly WordleDbContext _context;

    public GamesController(WordleDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    [HttpPost("start")]
    public async Task<IActionResult> StartGame()
    {
        var userId = GetUserId();
        var targetWord = WordProvider.GetRandomWord();
        var existing = await _context.Games
        .FirstOrDefaultAsync(g => g.UserId == userId && g.EndDate == null);

        if (existing != null)
            return BadRequest("You already have an active game.");

        var game = new Game
        {
            UserId = userId,
            TargetWord = targetWord,
            StartDate = DateTime.UtcNow,
            Attempts = 0,
            IsWin = false
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return Ok(new GameStartDTO
        {
            Id = game.Id,
            StartDate = game.StartDate
        });
    }

    [HttpPost("guess")]
    public async Task<IActionResult> Guess([FromBody] GuessRequestDTO guessInput)
    {
        var userId = GetUserId();

        var game = await _context.Games
            .Include(g => g.Guesses)
            .Where(g => g.UserId == userId && g.EndDate == null)
            .OrderByDescending(g => g.StartDate)
            .FirstOrDefaultAsync();

        if (game == null)
            return BadRequest("No active game found. Start a new game first.");

        if (game.IsWin || game.Attempts >= 6)
            return BadRequest("Game over.");

        if (guessInput.Word.Length != 5)
            return BadRequest("Guess must be 5 letters.");

        game.Attempts++;

        string result = EvaluateGuess(guessInput.Word, game.TargetWord);

        var guess = new Guess
        {
            GameId = game.Id,
            Word = guessInput.Word.ToLower(),
            GuessNumber = game.Attempts,
            GuessResult = result
        };

        game.Guesses.Add(guess);

        var stat = await _context.Statistics.FirstOrDefaultAsync(s => s.UserId == userId);

        if (guessInput.Word.ToLower() == game.TargetWord)
        {
            game.IsWin = true;
            game.EndDate = DateTime.UtcNow;

            if (stat != null)
            {
                stat.GamesPlayed++;
                stat.Wins++;
                stat.CurrentStreak++;
                stat.MaxStreak = Math.Max(stat.MaxStreak, stat.CurrentStreak);
                stat.TotalPoints += Math.Max(0, 10 - (game.Attempts - 1) * 2);
            }
        }
        else if (game.Attempts == 6)
        {
            game.EndDate = DateTime.UtcNow;

            if (stat != null)
            {
                stat.GamesPlayed++;
                stat.CurrentStreak = 0;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new GuessResultDTO
        {
            Word = guess.Word,
            GuessNumber = guess.GuessNumber,
            GuessResult = guess.GuessResult
        });
    }


    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        var userId = GetUserId();
        var games = await _context.Games
            .Where(g => g.UserId == userId)
            .Select(g => new GameSummaryDTO
            {
                Id = g.Id,
                IsWin = g.IsWin,
                Attempts = g.Attempts,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
                TargetWord = g.EndDate != null ? g.TargetWord : null
            })
            .ToListAsync();

        return Ok(games);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetGame(int id)
    {
        var userId = GetUserId();
        var game = await _context.Games
            .Include(g => g.Guesses)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (game == null)
            return NotFound("Game not found or access denied.");

        var dto = new GameDTO
        {
            Id = game.Id,
            StartDate = game.StartDate,
            EndDate = game.EndDate,
            Attempts = game.Attempts,
            IsWin = game.IsWin,
            TargetWord = game.EndDate != null ? game.TargetWord : null,
            Guesses = game.Guesses.Select(g => new GuessDTO
            {
                GuessNumber = g.GuessNumber,
                Word = g.Word,
                GuessResult = g.GuessResult
            }).ToList()
        };

        return Ok(dto);
    }


    private string EvaluateGuess(string guess, string target)
    {
        guess = guess.ToLower();
        target = target.ToLower();
        var result = new string[5];
        var used = new bool[5];

        for (int i = 0; i < 5; i++)
        {
            if (guess[i] == target[i])
            {
                result[i] = "correct";
                used[i] = true;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (result[i] == null)
            {
                bool found = false;
                for (int j = 0; j < 5; j++)
                {
                    if (!used[j] && guess[i] == target[j])
                    {
                        result[i] = "present";
                        used[j] = true;
                        found = true;
                        break;
                    }
                }
                if (!found) result[i] = "absent";
            }
        }

        return string.Join(",", result);
    }
}
