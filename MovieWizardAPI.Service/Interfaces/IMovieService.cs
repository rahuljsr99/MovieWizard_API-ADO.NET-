using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface IMovieService
    {
        Task<List<MovieRequest>> GetAllMovies();
        Task<List<MovieResponseForGrid>> GetAllMoviesForGrid();
        Task<int> AddMovie(MovieRequest movieRequest);
        Task<int> GetMovieIdByName(string movieName);
        Task<MovieMetrics> GetMovieMetrics();

    }
}
