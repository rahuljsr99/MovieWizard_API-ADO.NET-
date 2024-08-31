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
    public class MovieControllerTests
    {
        private readonly MovieController _movieController;
        private readonly Mock<IMovieService> _mockMovieService;

        public MovieControllerTests()
        {
            _mockMovieService = new Mock<IMovieService>();
            _movieController = new MovieController(_mockMovieService.Object);
        }

        [Fact]
        public async Task GetAllMovies_ReturnsOkResult_WithListOfMovies()
        {
            //arrange
            var movies = new List<MovieRequest>
            {
                new MovieRequest { MovieID = 1, Title= "Movie1"},
                new MovieRequest{MovieID = 2, Title= "Title"}
            };

            _mockMovieService.Setup(service => service.GetAllMovies()).ReturnsAsync(movies);

            var result = await _movieController.GetAllMovies();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<MovieRequest>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

        }

        [Fact]
        public async Task AddMovie_NullMovieRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _movieController.AddMovie(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request", badRequestResult.Value);
        }
        [Fact]
        public async Task AddMovie_InsertSuccess_ReturnsOk()
        {
            // Arrange
            var movieRequest = new MovieRequest { MovieID = 1, Title = "Movie 1" };
            _mockMovieService.Setup(service => service.AddMovie(movieRequest))
                             .ReturnsAsync(1); // Assuming 1 indicates success

            // Act
            var result = await _movieController.AddMovie(movieRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Movie inserted successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddMovie_InsertFailure_ReturnsBadRequest()
        {
            // Arrange
            var movieRequest = new MovieRequest { MovieID = 1, Title = "Movie 1" };
            _mockMovieService.Setup(service => service.AddMovie(movieRequest))
                             .ReturnsAsync(0); // Assuming 0 indicates failure

            // Act
            var result = await _movieController.AddMovie(movieRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to insert any record.", badRequestResult.Value);
        }
    }
}
