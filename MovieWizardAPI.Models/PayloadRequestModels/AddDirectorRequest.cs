using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class AddDirectorRequest
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public byte[]? ProfilePicture { get; set; }
    }
}
