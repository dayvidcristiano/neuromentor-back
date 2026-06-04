using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeuroMentor.Api.Data;
using NeuroMentor.Api.Models;

namespace NeuroMentor.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController(AppDbContext db) : ControllerBase
{
    private bool IsAdmin => User.FindFirstValue("isAdmin") == "True";
    private Guid MyId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── Users ──────────────────────────────────────────────────────────────

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? search = null)
    {
        if (!IsAdmin) return Forbid();

        var query = db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Name.ToLower().Contains(search.ToLower()) ||
                                     u.Email.ToLower().Contains(search.ToLower()));

        var users = await query
            .OrderBy(u => u.Name)
            .Select(u => new
            {
                u.Id, u.Name, u.Email,
                Role = u.Role.ToString().ToLower(),
                u.PhotoUrl, u.IsAiEnabled, u.IsAdmin, u.CreatedAt,
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest req)
    {
        if (!IsAdmin) return Forbid();

        var email = req.Email.Trim().ToLower();
        if (await db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { error = "E-mail já cadastrado." });

        if (req.Password.Length < 6)
            return BadRequest(new { error = "Senha deve ter pelo menos 6 caracteres." });

        var user = new User
        {
            Name = req.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = UserRole.Teacher,
            IsAiEnabled = true,
            IsAdmin = true,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { user.Id, user.Name, user.Email, user.IsAdmin, user.IsAiEnabled });
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (!IsAdmin) return Forbid();
        if (id == MyId) return BadRequest(new { error = "Você não pode deletar sua própria conta." });

        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Ok(new { deleted = id });
    }

    [HttpPut("users/{id:guid}/ai-access")]
    public async Task<IActionResult> SetAiAccess(Guid id, [FromBody] SetAiAccessRequest req)
    {
        if (!IsAdmin) return Forbid();

        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.IsAiEnabled = req.Enabled;
        await db.SaveChangesAsync();

        return Ok(new { id = user.Id, isAiEnabled = user.IsAiEnabled });
    }

    // ── Lessons / PDFs ─────────────────────────────────────────────────────

    [HttpGet("lessons")]
    public async Task<IActionResult> GetLessons()
    {
        if (!IsAdmin) return Forbid();

        var lessons = await db.Lessons
            .Include(l => l.Teacher)
            .Include(l => l.Modules)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.Title,
                l.SourceFileName,
                l.CreatedAt,
                TeacherName = l.Teacher.Name,
                TeacherEmail = l.Teacher.Email,
                ModuleCount = l.Modules.Count,
                RawTextSize = l.RawText.Length,
                HasRawText = l.RawText.Length > 0,
            })
            .ToListAsync();

        return Ok(lessons);
    }

    [HttpDelete("lessons/{id:guid}/raw-text")]
    public async Task<IActionResult> ClearRawText(Guid id)
    {
        if (!IsAdmin) return Forbid();

        var lesson = await db.Lessons.FindAsync(id);
        if (lesson is null) return NotFound();

        lesson.RawText = "";
        await db.SaveChangesAsync();

        return Ok(new { id = lesson.Id, cleared = true });
    }
}

public record SetAiAccessRequest(bool Enabled);
public record CreateAdminRequest(string Name, string Email, string Password);
