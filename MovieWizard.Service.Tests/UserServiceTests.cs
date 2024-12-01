using Xunit;
using MovieWizardAPI.Service;
using Moq;
using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;

namespace MovieWizard.Service.Tests
{
    public class UserServiceTests
    {
        private readonly UserService _movieService;
        private Mock<IUserRepository> _mockUserRepository;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _movieService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            //Arrange
            var userList = new List<User>()
            {
                new User { Username = "user1", Age = 1 },
                new User { Username = "user2", Age = 2 }
            };

            //SetupMock
            _mockUserRepository.Setup(repository => repository.GetAllUsersAsync()).ReturnsAsync(userList);

            //Act

            var result = await _movieService.GetAllUsers();

            //Assert 
            Assert.NotNull(result);
            Assert.Equal(userList, result);
        }

        [Theory]
        [InlineData(User addUserRequest)]
        public async Task AddUser_Successful()
        {
            //Arrange

            var user = new User
            {
                Age = 1,
                Bio = "Some Bio",
                CreatedAt = DateTime.Now,
                CreatedBy = "Test case",
                DateOfBirth = DateTime.Now,
                Email = "example@test.com",
                IsActive = true,
                Nationality = "UKWN",
                PasswordHash = "someHashedPassword",
                Phone = 1020202,
                Role = "User",
                UpdatedAt = DateTime.Now,
                UpdatedBy = "Test case",
                Username = "testUserName",
            };

            //SetupMock 
            _mockUserRepository.Setup(repository => repository.AddUserAsync(user)).ReturnsAsync(1);

            //Act

            var result = await _movieService.AddUser(user);

            Assert.NotNull(result);
            Assert.Equal(1, result);
        }
        
    }
}