using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;

namespace StacktimApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly StacktimDbContext _context;

    public TeamsController(StacktimDbContext context)
    {
        _context = context;
    }

    // GET /api/teams
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
    {
        var teams = await _context.Teams
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                Tag = t.Tag,
                CaptainId = t.CaptainId,
                CreationDate = t.CreationDate
            })
            .ToListAsync();

        return Ok(teams);
    }

    // GET /api/teams/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetTeam(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team == null)
            return NotFound();

        var dto = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag,
            CaptainId = team.CaptainId,
            CreationDate = team.CreationDate
        };

        return Ok(dto);
    }

    // POST /api/teams
    [HttpPost]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Unicité Name
        if (await _context.Teams.AnyAsync(t => t.Name == createDto.Name))
            return BadRequest($"Le nom d'équipe '{createDto.Name}' est déjà utilisé.");

        // Unicité Tag
        if (await _context.Teams.AnyAsync(t => t.Tag == createDto.Tag))
            return BadRequest($"Le tag '{createDto.Tag}' est déjà utilisé.");

        // Vérifier que Captain existe
        var captain = await _context.Players.FindAsync(createDto.CaptainId);
        if (captain == null)
            return BadRequest($"Le joueur avec Id {createDto.CaptainId} (CaptainId) n'existe pas.");

        // Création de l'équipe
        var team = new Team
        {
            Name = createDto.Name,
            Tag = createDto.Tag,
            CaptainId = createDto.CaptainId
            // CreationDate sera géré par la DB (HasDefaultValueSql), mais si tu veux, tu peux fixer DateTime.UtcNow ici
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Optionnel : ajouter l'entrée dans TeamPlayers pour le capitaine (role = 0)
        var captainRelation = new TeamPlayer
        {
            TeamId = team.Id,
            PlayerId = createDto.CaptainId,
            Role = 0 // Captain
            // JoinDate par défaut DB
        };
        _context.TeamPlayers.Add(captainRelation);
        await _context.SaveChangesAsync();

        var dto = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag,
            CaptainId = team.CaptainId,
            CreationDate = team.CreationDate
        };

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, dto);
    }

    // GET /api/teams/{id}/roster
    [HttpGet("{id}/roster")]
    public async Task<ActionResult<TeamRosterDto>> GetRoster(int id)
    {
        var team = await _context.Teams
            .Include(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null)
            return NotFound();

        var rosterPlayers = team.TeamPlayers
            .Select(tp => new TeamRosterPlayerDto
            {
                PlayerId = tp.PlayerId,
                Pseudo = tp.Player?.Pseudo ?? "Unknown",
                Role = tp.Role,
                JoinDate = tp.JoinDate
            })
            .OrderByDescending(p => p.Role) // optionally order (captain first if Role=0 you may want inverse)
            .ToList();

        var roster = new TeamRosterDto
        {
            TeamId = team.Id,
            TeamName = team.Name,
            Tag = team.Tag,
            Players = rosterPlayers
        };

        return Ok(roster);
    }
}
