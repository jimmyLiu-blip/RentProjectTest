using RentProject.Repository;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public class TestLocationService : ITestLocationService
    {
        private readonly ITestLocationRepository _testLocationRepository;

        public TestLocationService(ITestLocationRepository testLocationRepository)
        {
            _testLocationRepository = testLocationRepository;
        }

        public List<TestLocationLookupRow> GetTestLocationLookup()
        {
            return _testLocationRepository.GetTestLocationLookup();
        }
    }
}
