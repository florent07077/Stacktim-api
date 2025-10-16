using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StacktimApi.Data;
using StacktimApi.Models;
using System;

namespace StacktimApi.Tests.Helpers
{
    public class TestDbContextFactory
    {
        public static StacktimDbContext CreateContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<StacktimDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            var context = new StacktimDbContext(options);

            context.Players.AddRange(
                new Player { Id = 1, Pseudo = "PlayerOne", Email = "one@test.com", Rank = "Gold" },
                new Player { Id = 2, Pseudo = "PlayerTwo", Email = "two@test.com", Rank = "Silver" }
            );

            context.Teams.AddRange(
                new Team { Id = 1, Name = "TeamA", Tag = "TA" },
                new Team { Id = 2, Name = "TeamB", Tag = "TB" }
            );

            context.SaveChanges();

            return context;
        }

    }
}
