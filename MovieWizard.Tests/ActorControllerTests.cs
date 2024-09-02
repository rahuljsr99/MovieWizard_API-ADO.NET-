using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizard_API_ADO;
using MovieWizard_API_ADO.NET_.Controllers;
using MovieWizardAPI.Service.Interfaces;
using Moq;
using MovieWizardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Frameworks;

namespace MovieWizardController.Tests
{
    public class ActorControllerTests
    {
        private readonly ActorController _actorController;
        private readonly Mock<IActorService> _mockActorService;

        public ActorControllerTests()
        {
            _mockActorService = new Mock<IActorService>();
            _actorController = new ActorController(_mockActorService.Object);
        }
        [Fact]
        public async Task GetAllActors_ReturnsListOfActors()
        {
            var allActorsList = new List<Actor>()
            {
                new Actor { ActorID = 1, Name="Leonardo De Caprio" },
                new Actor { ActorID = 2, Name="Alexandra Dadario" }
            };

            _mockActorService.Setup(service => service.GetAllActors()).ReturnsAsync(allActorsList);

            var listOfActorsResult = await _actorController.GetAllActors();

            Assert.NotNull(listOfActorsResult);
           
            var okResult = Assert.IsType<OkObjectResult>(listOfActorsResult.Result);
            var returnValue = Assert.IsType<List<Actor>>(okResult.Value);
            Assert.Equal(allActorsList.Count(), returnValue.Count());
            Assert.Equal(allActorsList, returnValue);

        }

        [Fact]
        public async Task AddActor_ReturnsCreatedAtActionResult_WithCorrectActor()
        {
            // Arrange
            var newActor = new Actor
            {
                ActorID = 1,
                Name = "Leonardo DiCaprio",
                Bio = "Famous American actor.",
                DateOfBirth = new DateTime(1974, 11, 11),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                UpdatedBy = "Admin",
                Nationality = "American"
            };

            int actorId = 1;
            _mockActorService.Setup(service => service.AddActor(It.IsAny<Actor>())).ReturnsAsync(actorId);

            var insertResult = _actorController.AddActor(newActor);

            //Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(insertResult.Result);
            var returnValue = Assert.IsType<int>(createdAtActionResult.Value);

            Assert.Equal(actorId, returnValue);
        }
        }
}
