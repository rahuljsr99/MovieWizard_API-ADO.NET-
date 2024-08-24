using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class MovieRequest
    {
        public int MovieID { get; set; }
        public string? Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int[]? Genres { get; set; }
        public string? Description { get; set; }
        public byte[]? Poster { get; set; }
        public decimal? Budget { get; set; }
        public decimal? Revenue { get; set; }
        public double? IMDBRating { get; set; }
        public double? RottenTomatoesRating { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
