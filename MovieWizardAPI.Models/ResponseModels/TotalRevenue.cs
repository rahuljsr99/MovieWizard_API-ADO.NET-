using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class TotalRevenue
    {
        public decimal? TotalRevenueAmount { get; set; }
        public decimal? TotalRevenueFromMovies { get; set; }
        public decimal? TotalRevenueFromTVShows { get; set; }
    }
}
