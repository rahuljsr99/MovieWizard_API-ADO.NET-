using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class Actor
    {
        public int ActorID { get; set; }

        public string? Name { get; set; }

        public string? Bio { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsActive { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Nationality { get; set; }
    }
}

