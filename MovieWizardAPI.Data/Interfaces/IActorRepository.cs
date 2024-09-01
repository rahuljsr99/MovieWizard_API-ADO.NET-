using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface IActorRepository
    {
        Task<List<Actor>> GetAllActorsAsync();
        //Task<List<MovieGenresResponse>> GetAllMovieGenresAsync();
    }
}
