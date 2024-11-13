using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsersAsync(int pageNumber, int pageSize);

        Task<int> AddUserAsync(User addUserRequest);
     
        Task UpdateUserAsync(User updateUserRequest);
    }
}
