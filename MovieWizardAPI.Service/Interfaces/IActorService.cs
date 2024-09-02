using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface IActorService
    {
        Task<List<Actor>> GetAllActors();
        Task<int> AddActor(Actor actorRequest);
    }
}
