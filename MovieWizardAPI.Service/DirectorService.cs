using MovieWizardAPI.Models;
using MovieWizardAPI.Data;
using MovieWizardAPI.Service.Interfaces;
using MovieWizardAPI.Data.Interfaces;

namespace MovieWizardAPI.Service
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _directorRepository;

        public DirectorService(IDirectorRepository directorRepository)
        {
            _directorRepository = directorRepository;
        }
        public async Task<List<Director>> GetAllDirectors()
        {
            var queryResult = await _directorRepository.GetAllDirectorsAsync();
            var directors = queryResult.ToList();
            return directors;
        }
        public async Task<int> AddDirector(AddDirectorRequest addDirectorRequest)
        {
            var queryResult = await _directorRepository.AddDirectorAsync(addDirectorRequest);
            return queryResult;
        }
    }
}
