using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;


namespace MovieWizardAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _usersRepository;

        public UserService(IUserRepository userRepository)
        {
            _usersRepository = userRepository;
        }
        public async Task<List<User>> GetAllUsers()
        {
            var queryResult = await _usersRepository.GetAllUsersAsync();
            var users = queryResult.ToList();
            return users;
        }
        public async Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsers(int pageNumber, int pageSize)
        {
            var queryResult = await _usersRepository.GetPagedUsersAsync(pageNumber, pageSize);

            return queryResult;
        }
        public async Task<int> AddUser(User addUserRequest)
        {
            var queryResult = await _usersRepository.AddUserAsync(addUserRequest);
            return queryResult;
        }   

        public async Task<User> GetUserByEmail(string email)
        {
           User user = await _usersRepository.GetUserByEmailAsync(email);
           return user;
        }
           
    }
}
