using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }
        [HttpGet("GetAllGenres")]
        public async Task<ActionResult<List<MovieGenre>>> GetAllGenres()
        {
           List <MovieGenre> allMovieGenres = await _genreService.GetAllGenres();
           return Ok(allMovieGenres);
        }
        [HttpGet("GetAllMovieGenres")]
        public async Task<ActionResult<List<MovieGenresResponse>>> GetAllMovieGenres()
        {
            List<MovieGenresResponse> allMovieGenres = await _genreService.GetAllMovieGenres();
            if(allMovieGenres.Count == 0)
                { return BadRequest($"No records"); }
            return Ok(allMovieGenres);
        }

    }
}
