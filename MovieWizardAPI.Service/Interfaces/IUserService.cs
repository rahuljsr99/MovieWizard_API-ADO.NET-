using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsers(int pageNumber, int pageSize);
        Task<int> AddUser(User addUserRequest);
    }
}
