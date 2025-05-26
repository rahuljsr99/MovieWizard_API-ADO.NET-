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
        public async Task<IActionResult> GetMovieById(int movieId)
        {
            if(movieId == 0)
            {
                return BadRequest("MovieId is 0");
            }
            else
            {

            }
        }
        [HttpPatch("UpdateUserPartial")]
        public async Task<IActionResult> UpdateMovieDataPartial([FromBody] UpdateMoviePartial movieUpdate)
        {
            if (movieUpdate == null || movieUpdate.UserID <= 0)
                return BadRequest("Invalid movie data.");

            var user = await _movieService.GetMovieById(movieUpdate.UserID);
            if (user == null)
                return NotFound("User not found.");
            if (user.IsActive)
                userUpdate.IsActive = true;
            else
                userUpdate.IsActive = false;
            var result = await _userService.UpdateUser(userUpdate);
            if (result)
                return Ok("Updated");
            return BadRequest("Error in updating data");

        }

        [Authorize]
        [HttpGet("TestAuth")]
        public IActionResult TestAuth()
        {
            return Ok("Authentication successful!");
        }
    }
}
