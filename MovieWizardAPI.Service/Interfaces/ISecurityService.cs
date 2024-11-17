using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieWizardAPI.Models;

namespace MovieWizardAPI.Service.Interfaces
{
    public interface ISecurityService
    {
        Task<ValidateUserResponse> ValidateUserCredentials(string email, string password);
    }
}
