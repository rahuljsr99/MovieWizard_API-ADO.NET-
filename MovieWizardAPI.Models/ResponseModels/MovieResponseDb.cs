Musing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieWizardAPI.Models
{
    public class MovieDataAsIdsDb
    {
        public int MovieId { get; set; }
        public int DirectorId {  get; set; }
        public List<int> GenreIds { get; set; }
        public List<int> ActorIds { get; set; }
    }
}
