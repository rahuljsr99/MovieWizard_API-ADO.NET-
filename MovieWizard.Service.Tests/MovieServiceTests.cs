using Xunit;
using MovieWizardAPI.Service;
using Moq;
using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;

namespace MovieWizard.Service.Tests
{
    public class MovieServiceTests
    {
        private readonly MovieService _movieService;
        private Mock<IMovieRepository> _mockMovieRepository;

        public MovieServiceTests()
        {
            _mockMovieRepository = new Mock<IMovieRepository>();
            _movieService = new MovieService(_mockMovieRepository.Object);
        }

        [Fact]
        public async Task GetAllMovies_ReturnsListOfMovies()
        {
            //arrange
            var movies = new List<MovieRequest>
            {
                new MovieRequest { MovieID = 1, Title = "Movie1"},
                new MovieRequest { MovieID = 2, Title = "Movie2"}
            };

            _mockMovieRepository.Setup(repository => repository.GetAllMoviesAsync()).ReturnsAsync(movies);

            //act
            var result = await _movieService.GetAllMovies();

            //assert

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

    }
}