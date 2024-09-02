using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;
        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet("GetAllActors")]
        public async Task<ActionResult<List<Actor>>> GetAllActors()
        {

            var allActorsList = await _actorService.GetAllActors();
            return Ok(allActorsList);
        }

        [HttpPost("AddActor")]
        public async Task<ActionResult> AddActor(Actor addActorRequest)
        {

            var newActorActionResult = await _actorService.AddActor(addActorRequest);
            return CreatedAtAction(nameof(AddActor), newActorActionResult);

        }
    }
}
