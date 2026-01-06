using RentProject.Shared.DTO;

namespace RentProject.Repository
{
    public interface ITestLocationRepository
    {
        List<TestLocationLookupRow> GetTestLocationLookup();
    }
}
