using RentProject.Domain;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public interface IRentTimeService
    {
        string TestConnection();

        CreateRentTimeResult CreateRentTime(RentTime model);

        List<RentTimeListRow> GetProjectViewList();

        RentTime GetRentTimeById(int rentTimeId);

        void UpdateRentTimeById(RentTime model);

        void DeletedRentTime(int rentTimeId, int modifiedByUserId);
    }
}
