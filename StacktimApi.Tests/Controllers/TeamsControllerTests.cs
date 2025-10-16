using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class TeamsControllerTests
    {
        [Fact]
        public async Task GetTeams_ReturnsAllTeams()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var result = await controller.GetTeams();
            var okResult = result.Result as OkObjectResult;
            var teams = okResult?.Value as List<TeamDto>;

            teams.Should().NotBeNull();
            teams.Should().HaveCount(2); 
        }

        [Fact]
        public async Task GetTeam_WithValidId_ReturnsTeam()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var result = await controller.GetTeam(1);
            var okResult = result.Result as OkObjectResult;
            var team = okResult?.Value as TeamDto;

            team.Should().NotBeNull();
            team.Name.Should().Be("TeamA");
        }

        [Fact]
        public async Task GetTeam_WithInvalidId_ReturnsNotFound()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var result = await controller.GetTeam(999);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateTeam_WithValidData_ReturnsCreated()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var newTeam = new CreateTeamDto
            {
                Name = "TeamC",
                Tag = "TC",
                CaptainId = 1
            };

            var result = await controller.CreateTeam(newTeam);
            var createdResult = result.Result as CreatedAtActionResult;
            var teamDto = createdResult?.Value as TeamDto;

            teamDto.Should().NotBeNull();
            teamDto.Name.Should().Be("TeamC");
        }

        [Fact]
        public async Task CreateTeam_WithDuplicateName_ReturnsBadRequest()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var duplicateTeam = new CreateTeamDto
            {
                Name = "TeamA",
                Tag = "TX",
                CaptainId = 1
            };

            var result = await controller.CreateTeam(duplicateTeam);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateTeam_WithDuplicateTag_ReturnsBadRequest()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var duplicateTeam = new CreateTeamDto
            {
                Name = "TeamX",
                Tag = "TA",
                CaptainId = 1
            };

            var result = await controller.CreateTeam(duplicateTeam);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateTeam_WithInvalidCaptainId_ReturnsBadRequest()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var invalidCaptainTeam = new CreateTeamDto
            {
                Name = "TeamY",
                Tag = "TY",
                CaptainId = 999
            };

            var result = await controller.CreateTeam(invalidCaptainTeam);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetRoster_WithValidId_ReturnsRoster()
        {
            using var context = TestDbContextFactory.CreateContext();

            
            context.TeamPlayers.Add(new TeamPlayer
            {
                TeamId = 1,
                PlayerId = 1,
                Role = 0
            });
            context.SaveChanges();

            var controller = new TeamsController(context);

            var result = await controller.GetRoster(1);
            var okResult = result.Result as OkObjectResult;
            var roster = okResult?.Value as TeamRosterDto;

            roster.Should().NotBeNull();
            roster.Players.Should().HaveCount(1);
            roster.Players.First().Pseudo.Should().Be("PlayerOne");
        }

        [Fact]
        public async Task GetRoster_WithInvalidId_ReturnsNotFound()
        {
            using var context = TestDbContextFactory.CreateContext();
            var controller = new TeamsController(context);

            var result = await controller.GetRoster(999);
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
