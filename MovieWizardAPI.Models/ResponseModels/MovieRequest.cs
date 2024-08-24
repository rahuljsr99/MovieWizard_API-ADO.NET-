using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class MovieGenresResponse
    {
        public int MovieID { get; set; }
        
        public int GenreId {  get; set; }

        public string? GenreName {  get; set; }
    }
}
