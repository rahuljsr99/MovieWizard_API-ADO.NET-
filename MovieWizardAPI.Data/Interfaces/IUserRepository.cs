using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;
using MovieWizardAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;

namespace MovieWizardAPI.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsersAsync(int pageNumber, int pageSize);

        Task<int> AddUserAsync(User addUserRequest);
     
        Task<User> GetUserByEmailAsync(string email);
        Task<SoulCounts> GetSoulCountsAsync();

        Task<bool> UpdateUserAsync(UpdateUserDTO userUpdate);
        Task<User> GetUserByIdAsync(int userId);
    }
}
