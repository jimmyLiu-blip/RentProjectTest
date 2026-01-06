using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public interface IProjectService
    {
        List<ProjectLookupRow> GetProjectLookup();
    }
}
