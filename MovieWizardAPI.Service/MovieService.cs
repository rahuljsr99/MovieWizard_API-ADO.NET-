using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;


namespace MovieWizardAPI.Service
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }
        public async Task<List<MovieRequest>> GetAllMovies()
        {
            var queryResult = await _movieRepository.GetAllMoviesAsync();
            var movies = queryResult.ToList();
            return movies;
        }
        public async Task<int> AddMovie(MovieRequest movieRequest)
        {
            var queryResult = await _movieRepository.AddMovieAsync(movieRequest);
            return queryResult;
        }   
           
    }
}
