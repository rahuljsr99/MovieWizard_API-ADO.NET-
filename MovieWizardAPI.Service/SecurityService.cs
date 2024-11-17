using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizardAPI.Service
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserService _userService;

        public SecurityService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ValidateUserResponse> ValidateUserCredentials(string email, string password)
        {
            User user = await _userService.GetUserByEmail(email);
            if (user != null)
            {
                if (user.PasswordHash != null)
                {
                    if (user.PasswordHash == password)
                    {
                        ValidateUserResponse validateUserResponse = new ValidateUserResponse()
                        {
                            IsUserAuthenticated = true,
                            IsUserAuthorized = true,
                            message = "Login permitted",
                            statusCode = 200
                        };
                        if (string.IsNullOrEmpty(user.Role))
                        {
                            user.Role = "User";
                            validateUserResponse.Role = user.Role;
                        }
                        else
                        {
                            validateUserResponse.Role = user.Role;
                        }
                        return validateUserResponse;
                    }
                    else {
                        ValidateUserResponse validateUserResponse = new ValidateUserResponse()
                        {
                            IsUserAuthenticated = false,
                            IsUserAuthorized = false,
                            message = "Incorrect password",
                            statusCode = 401
                        };
                        return validateUserResponse;
                    }
                }
                else
                {
                    ValidateUserResponse validateUserResponse = new ValidateUserResponse()
                    {
                        IsUserAuthenticated = false,
                        IsUserAuthorized = false,
                        message = "Password is null in DB",
                        statusCode = 401
                    };
                    return validateUserResponse;
                }
            }
            else
            {
                ValidateUserResponse validateUserResponse = new ValidateUserResponse()
                {
                    IsUserAuthenticated = false,
                    IsUserAuthorized = false,
                    message = "User not present",
                    statusCode = 401
                };
                return validateUserResponse;
            }
        }

}
}
