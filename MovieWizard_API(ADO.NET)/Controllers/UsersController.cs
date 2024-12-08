using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMemoryCache _memoryCache;

        public UsersController(IUserService userService, IMemoryCache memoryCache)
        {
            _userService = userService;
            _memoryCache = memoryCache;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {

            var cachedKey = "cachedKey";

            // Check if data is in the cache
            if (_memoryCache.TryGetValue(cachedKey, out var users))
            {
                // If data is found in cache, return it
                return Ok(users);
            }
            else
            {
                // If not found in cache, fetch from service
                var listOfAllUsers = await _userService.GetAllUsers();

                // Cache the data with expiration policy
                var cachedOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // Set absolute expiration (10 minutes)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                // Store the data in cache
                _memoryCache.Set(cachedKey, listOfAllUsers, cachedOptions);

                return Ok(listOfAllUsers);
            }

        }
        [HttpGet("GetPagedUsers")]
        public async Task<IActionResult> GetPagedUsers(int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                // Call the service method to fetch paginated users from the database
                var pagedUsers = await _userService.GetPagedUsers(pageNumber, pageSize);

                // Return a structured response with totalRecords and userList
                return Ok(new
                {
                    totalRecords = pagedUsers.totalRecords,
                    userList = pagedUsers.users
                });
            }
            catch (Exception ex)
            {
                // Handle any errors (optional)
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(User addUser)
        {
            try
            {
                var id = await _userService.AddUser(addUser);
                return Ok($"{addUser.Username} added with Id {id}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser(User updateUser)
        {
            try
            {
                await _userService.AddUser(updateUser);
                return Ok($"{updateUser.Username} updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
