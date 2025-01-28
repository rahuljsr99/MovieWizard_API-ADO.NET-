using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> GetAllDirectorsAsync();
        Task<int> AddDirectorAsync(AddDirectorRequest movieRequest);

        Task<int> GetDirectorIdByNameAsync(string directorName);
    }
}
