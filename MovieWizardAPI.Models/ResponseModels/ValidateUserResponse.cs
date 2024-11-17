using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class ValidateUserResponse
    {
        public int statusCode {  get; set; }
        public string message { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public bool IsUserAuthorized { get; set; }
        public string Role { get; set; }
    }
}
