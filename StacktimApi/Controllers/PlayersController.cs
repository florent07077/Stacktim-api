using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Data;
using StacktimApi.DTOs;
using StacktimApi.Models;

namespace StacktimApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly StacktimDbContext _context;

        public PlayersController(StacktimDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
        {
            var players = await _context.Players
                .Select(player => new PlayerDto
                {
                    Id = player.Id,
                    Pseudo = player.Pseudo,
                    Email = player.Email,
                    Rank = player.Rank,
                    TotalScore = player.TotalScore
                })
                .ToListAsync();

            return Ok(players);
        }

  
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            var playerDto = new PlayerDto
            {
                Id = player.Id,
                Pseudo = player.Pseudo,
                Email = player.Email,
                Rank = player.Rank,
                TotalScore = player.TotalScore
            };

            return Ok(playerDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto createPlayerDto)
        {
            // Vérification des doublons
            if (await _context.Players.AnyAsync(p => p.Pseudo == createPlayerDto.Pseudo))
                return BadRequest("Ce pseudo existe déjà.");
            if (await _context.Players.AnyAsync(p => p.Email == createPlayerDto.Email))
                return BadRequest("Cet email est déjà utilisé.");

            // Création du joueur
            var newPlayer = new Player
            {
                Pseudo = createPlayerDto.Pseudo,
                Email = createPlayerDto.Email,
                Rank = createPlayerDto.Rank,
                TotalScore = 0
            };

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            // DTO de retour
            var playerDto = new PlayerDto
            {
                Id = newPlayer.Id,
                Pseudo = newPlayer.Pseudo,
                Email = newPlayer.Email,
                Rank = newPlayer.Rank,
                TotalScore = newPlayer.TotalScore
            };

            return CreatedAtAction(nameof(GetPlayer), new { id = newPlayer.Id }, playerDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, UpdatePlayerDto updatePlayerDto)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            // Mise à jour des champs si présents dans le DTO
            if (!string.IsNullOrWhiteSpace(updatePlayerDto.Pseudo))
                player.Pseudo = updatePlayerDto.Pseudo;

            if (!string.IsNullOrWhiteSpace(updatePlayerDto.Email))
                player.Email = updatePlayerDto.Email;

            if (!string.IsNullOrWhiteSpace(updatePlayerDto.Rank))
                player.Rank = updatePlayerDto.Rank;

            if (updatePlayerDto.TotalScore.HasValue)
                player.TotalScore = updatePlayerDto.TotalScore.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetLeaderboard()
        {
            var leaderboard = await _context.Players
                .OrderByDescending(p => p.TotalScore)
                .Take(10)
                .Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Pseudo = p.Pseudo,
                    Email = p.Email,
                    Rank = p.Rank,
                    TotalScore = p.TotalScore
                })
                .ToListAsync();

            return Ok(leaderboard);
        }
    }
}
