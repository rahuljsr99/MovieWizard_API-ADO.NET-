using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models.ResponseModels
{
    public class SoulCounts
    {
        public int AdminCount { get; set; }
        public int UserCount { get; set; }
        public int TotalSoulCount { get; set; }
        public int ActiveAdminCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int InactiveAdminCount { get; set; }
        public int InactiveUserCount { get; set; }
    }

}
