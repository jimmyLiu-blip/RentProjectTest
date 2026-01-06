using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public interface ITestLocationService
    {
        List<TestLocationLookupRow> GetTestLocationLookup();
    }
}
