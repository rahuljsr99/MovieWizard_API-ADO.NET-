using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Models;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<ActionResult<List<MovieRequest>>> GetAllMovies()
        {
            var movieList = await _movieService.GetAllMovies();
            return Ok(movieList);
        }

        [HttpPost("AddMovie")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddMovie(MovieRequest movieRequest)
        {
            if (movieRequest == null)
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

        [Authorize]
        [HttpGet("TestAuth")]
        public IActionResult TestAuth()
        {
            return Ok("Authentication successful!");
        }
    }
}
