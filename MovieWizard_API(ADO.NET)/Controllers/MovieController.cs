using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Models;
using Microsoft.AspNetCore.Authorization;
using MovieWizardAPI.Service;

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
            var movieList =  _movieService.GetAllMovies().Result;
            return Ok(movieList);
        }
        [Obsolete]
        [HttpGet("GetAllMoviesForGrid")]
        public async Task<ActionResult<List<MovieRequest>>> GetAllMoviesForGrid()
        {
            var movieList = _movieService.GetAllMoviesForGrid().Result;
            if (movieList == null)
            {
                return BadRequest("movieList null");
            }
            else
            {
                return Ok(movieList);
            }

        }

        [HttpPost("AddMovie")]
        public async Task<ActionResult> AddMovie(MovieRequest movieRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Invalid request");
            }

            var result = await _movieService.AddMovie(movieRequest);
            if (result == 0)
            {
                return BadRequest("Failed to insert any record.");
            }
            else
            {
                return Ok("Movie inserted successfully.");
            }
            
        }
        [HttpGet("GetMovieMetrics")]
        public async Task<IActionResult> GetMoviesMetrics()
        {
            var result = await _movieService.GetMovieMetrics();
            if (result == null)
            {
                return BadRequest("No data");
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet("SearchAndGetMovies")]
        public async Task<ActionResult<List<MovieResponseForGrid>>> SearchMovies([FromQuery] string? term = "")
        {
            if (string.IsNullOrEmpty(term))
            {
                var allMovies = await _movieService.GetAllMovies();
                return Ok(allMovies);
            }

            var result = await _movieService.SearchMovies(term);
            if (result == null || !result.Any())
            {
                return BadRequest("No data");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("TestAuth")]
        public IActionResult TestAuth()
        {
            return Ok("Authentication successful!");
        }
    }
}
