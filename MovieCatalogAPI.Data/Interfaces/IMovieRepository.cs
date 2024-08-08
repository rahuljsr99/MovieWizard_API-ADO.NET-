using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieCatalogAPI.Models;

namespace MovieCatalogAPI.Data.Interfaces
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAllMovies();
    }
}
