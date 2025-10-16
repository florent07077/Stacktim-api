using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StacktimApi.Controllers;
using StacktimApi.DTOs;
using StacktimApi.Models;
using StacktimApi.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StacktimApi.Tests.Controllers
{
    public class PlayersControllerTests
    {
        [Fact]
        public async Task GetPlayers_ReturnsAllPlayers()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var actionResult = await controller.GetPlayers();
            var okResult = actionResult.Result as OkObjectResult;
            var players = okResult?.Value as List<PlayerDto>;

            players.Should().NotBeNull();
            players.Should().HaveCount(2); 
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsPlayer()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var actionResult = await controller.GetPlayer(1);
            var okResult = actionResult.Result as OkObjectResult;
            var player = okResult?.Value as PlayerDto;

            player.Should().NotBeNull();
            player.Pseudo.Should().Be("PlayerOne");
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var actionResult = await controller.GetPlayer(999);

            actionResult.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreatePlayer_WithValidData_ReturnsCreated()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var newPlayer = new CreatePlayerDto
            {
                Pseudo = "NewPlayer",
                Email = "new@test.com",
                Rank = "Bronze"
            };

            var result = await controller.CreatePlayer(newPlayer);
            var createdResult = result.Result as CreatedAtActionResult;
            var playerDto = createdResult?.Value as PlayerDto;

            playerDto.Should().NotBeNull();
            playerDto.Pseudo.Should().Be("NewPlayer");
        }

        [Fact]
        public async Task CreatePlayer_WithDuplicatePseudo_ReturnsBadRequest()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var duplicatePlayer = new CreatePlayerDto
            {
                Pseudo = "PlayerOne",
                Email = "duplicate@test.com",
                Rank = "Bronze"
            };

            var result = await controller.CreatePlayer(duplicatePlayer);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeletePlayer_WithValidId_ReturnsNoContent()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var result = await controller.DeletePlayer(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsOrderedPlayers()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new PlayersController(context);

            var actionResult = await controller.GetLeaderboard();
            var okResult = actionResult.Result as OkObjectResult;
            var leaderboard = okResult?.Value as List<PlayerDto>;

            leaderboard.Should().NotBeNull();
            leaderboard.Select(p => p.TotalScore).Should().BeInDescendingOrder();
        }
    }
}
