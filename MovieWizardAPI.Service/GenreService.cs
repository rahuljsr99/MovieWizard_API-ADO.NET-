using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;

namespace MovieWizardAPI.Service
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }


        public async Task<List<MovieGenre>> GetAllGenres()
        {
            var queryResult = await _genreRepository.GetAllGenresAsync();
            List<MovieGenre> allMovieGenres = queryResult.ToList();
            return allMovieGenres;
        }
    }
}
