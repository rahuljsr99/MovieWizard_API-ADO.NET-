using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;

namespace MovieWizardAPI.Service
{
    public class MovieService : IMovieService
    {
        private readonly IMovieService _movieService;
        private readonly IMovieRepository _movieRepository;

        MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }


        public async Task<List<Movie>> GetAllMovies()
        {
            var queryResult = await _movieRepository.GetAllMoviesAsync();
            List<Movie> movies = queryResult.ToList();
            return movies;
        }
    }
}
