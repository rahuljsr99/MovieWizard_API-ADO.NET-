using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Models;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("GetAllMovies")]
        public async Task<ActionResult<List<MovieRequest>>> GetAllMovies()
        {
            List<MovieRequest> movieList = await _movieService.GetAllMovies();
            return Ok(movieList);
        }

        [HttpPost("AddMovie")]
        public async Task<ActionResult> AddMovie()
    }
}
