using RentProject.Shared.DTO;

namespace RentProject.Repository
{
    public interface IProjectRepository
    {
        List<ProjectLookupRow> GetProjectLookup();
    }
}
