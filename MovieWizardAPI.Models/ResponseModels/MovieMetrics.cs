using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class MovieMetrics
    {
        public int TotalMovies { get; set; }
        public int TotalActiveMovies { get; set; }
        public int TotalInactiveMovies { get; set; }
        public List<string> TopFiveHighestSellingMovies { get; set; }
        public List<string> LeastFiveSellingMovies { get; set; }
    }
}
