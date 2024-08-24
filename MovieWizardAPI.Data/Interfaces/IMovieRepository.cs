using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<MovieRequest>> GetAllMoviesAsync();
        Task<int> AddMovieAsync(MovieRequest movieRequest);
    }
}
