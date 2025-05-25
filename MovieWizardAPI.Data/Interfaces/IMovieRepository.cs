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
        Task<List<MovieResponseForGrid>> GetAllMoviesForGrid();
        Task<int> AddMovieAsync(MovieRequest movieRequest);
        Task<int> GetMovieIdByNameAsync(string movieName);

        Task<MovieMetrics> GetMovieMetricsAsync();
    }
}
