using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Models;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DirectorController : Controller
    {
        private readonly IDirectorService _directorService;

        public DirectorController(IDirectorService directorService)
        {
            _directorService = directorService;
        }
        [HttpGet("GetAllDirectors")]
        public async Task<ActionResult<List<Director>>> GetAllDirectors()
        {
            return await _directorService.GetAllDirectors();
        }

        [HttpPost("AddDirector")]
        public async Task<ActionResult> AddDirector(AddDirectorRequest addDirectorRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Input to endpoint not valid.");
            }

            await _directorService.AddDirector(addDirectorRequest);
            return Ok("1 Director added");
        }
    }
}
