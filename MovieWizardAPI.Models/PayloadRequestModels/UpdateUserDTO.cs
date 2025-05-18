using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class UpdateUserDTO
    {
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; } // Changed Name to Username for consistency
        public string? PasswordHash { get; set; } // Added PasswordHash
        public string? Email { get; set; }
        public int? Phone { get; set; }
        public int? Age { get; set; }
        public string? Bio { get; set; } // Assuming you want to add this to the DB
        public DateTime DateOfBirth { get; set; } // Added DateOfBirth

        public bool IsActive { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? Nationality { get; set; }
        public string? Role { get; set; } // Added Role to match the DB column
    }
}
