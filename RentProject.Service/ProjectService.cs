using RentProject.Repository;
using RentProject.Shared.DTO;
using RentProject.UIModels;

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
