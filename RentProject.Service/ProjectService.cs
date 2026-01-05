using RentProject.Repository;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public class ProjectService
    {
        private readonly DapperProjectRepository _projectRepo;

        public ProjectService(DapperProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public List<ProjectLookupRow> GetProjectLookup()
        { 
            return _projectRepo.GetProjectLookup();
        }
    }
}
