using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : Controller
    {
        private readonly IActorService _actorService;
        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Actor>>> GetAllActors() {

           var allActorsList = await _actorService.GetAllActors();
            return Ok(allActorsList);
        }

    }
}
