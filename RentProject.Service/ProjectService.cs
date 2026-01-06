using RentProject.Repository;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepo;

        public ProjectService(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public List<ProjectLookupRow> GetProjectLookup()
        { 
            return _projectRepo.GetProjectLookup();
        }
    }
}
