using RentProject.Repository;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public class TestLocationService
    {
        private readonly DapperTestLocationRepository _testLocationRepository;

        public TestLocationService(DapperTestLocationRepository testLocationRepository)
        {
            _testLocationRepository = testLocationRepository;
        }

        public List<TestLocationLookupRow> GetTestLocationLookup()
        {
            return _testLocationRepository.GetTestLocationLookup();
        }
    }
}
