using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class MovieResponseForGrid
    {
        public int MovieID { get; set; }                 // Movie ID
        public string Title { get; set; }                // Movie Title
        public string Director { get; set; }             // Director Name
        public string Actors { get; set; }               // Actors Names (comma separated)
        public string Description { get; set; }          // Movie Description
        public string Genre { get; set; }                // Genre (optional)
        public decimal ImdbRating { get; set; }           // IMDb Rating
        public decimal RottenTomatoesRating { get; set; }    // Rotten Tomatoes Rating (%)
        public decimal Price { get; set; }               // Movie Price (Decimal for precision)
        public DateTime ReleaseDate { get; set; }        // Release Date
        public string Poster { get; set; }               // Movie Poster URL (optional)


    }
}
