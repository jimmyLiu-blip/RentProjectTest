using RentProject.Domain;
using RentProject.Shared.DTO;

namespace RentProject.Repository
{
    public interface IRentTimeRepository
    {
        string TestConnection();

        CreateRentTimeResult CreateRentTime(RentTime model);

        List<RentTimeListRow> GetActiveRentTimesForProjectView();

        RentTime? GetRentTimeById(int rentTimeId);

        int UpdateRentTime(RentTime model);

        int DeletedRentTime(int rentTimeId, int modifiedByUserId);
    }
}
