using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;

namespace MovieWizardAPI.Service
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;

        public ActorService(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<List<Actor>> GetAllActors()
        {
            List<Actor> allActorsList = new List<Actor>();
            allActorsList = await _actorRepository.GetAllActorsAsync() ;
            return allActorsList;
        }
    }
}
