using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class PagedUsersResponse
    {
        public int totalRecords { get; set; }
        public List<User> userList { get; set; }
    }
}
