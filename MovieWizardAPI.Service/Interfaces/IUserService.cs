using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;
using MovieWizardAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsers(int pageNumber, int pageSize);
        Task<int> AddUser(User addUserRequest);
        Task<User> GetUserByEmail(string email);
        Task<SoulCounts> GetSoulCountsAsync();

        Task<bool> UpdateUser(UpdateUserDTO user);
    }
}
