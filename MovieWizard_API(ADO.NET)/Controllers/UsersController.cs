using Microsoft.AspNetCore.Mvc;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var listOfAllUsers = await _userService.GetAllUsers();
            return Ok(listOfAllUsers);
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
