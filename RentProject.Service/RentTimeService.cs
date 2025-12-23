using RentProject.Domain;
using RentProject.Repository;

namespace RentProject.Service
{
    public class RentTimeService
    {
        private readonly DapperRentTimeRepository _repo;

        public RentTimeService(DapperRentTimeRepository repo)
        { 
            _repo = repo;
        }

        public CreateRentTimeResult CreateRentTime(RentTime model)
        {
            ValidateRequired(model);
            CalculateEstimated(model);

            return _repo.CreateRentTime(model);
        }

        private static void ValidateRequired(RentTime model)
        {
            if (string.IsNullOrWhiteSpace(model.Area)) throw new Exception("Area 必填");
            if (string.IsNullOrWhiteSpace(model.CreatedBy)) throw new Exception("CreatedBy 必填");
            if (string.IsNullOrWhiteSpace(model.ProjectName)) throw new Exception("ProjectName 必填");
            if (string.IsNullOrWhiteSpace(model.PE)) throw new Exception("PE 必填");
            if (string.IsNullOrWhiteSpace(model.ProjectNo)) throw new Exception("ProjectNo 必填");
            if (string.IsNullOrWhiteSpace(model.Location)) throw new Exception("Location 必填");
        }

        private static void CalculateEstimated(RentTime model)
        {
            if (model.StartDate is null || model.EndDate is null || model.StartTime is null || model.EndTime is null)
            { 
                model.EstimatedMinutes = 0;
                model.EstimatedHours = 0;
                return; //結束這個方法，不要在往下算
            }

            var start = model.StartDate.Value.Date + model.StartTime.Value;
            var end = model.EndDate.Value.Date + model.EndTime.Value;

            if (end < start) throw new Exception("結束時間不可早於開始時間");

            var minutes = (int)(end - start).TotalMinutes; // 轉換成總分鐘

            if (model.HasLunch) minutes -= model.LunchMinutes;
            if (model.HasDinner) minutes -= model.DinnerMinutes;

            if (minutes < 0) throw new Exception("扣除午餐/晚餐後，預估時間變成負數，請檢查時間與晚餐分配");

            model.EstimatedMinutes = minutes;
            model.EstimatedHours = Math.Round(minutes/60m, 2);
        }
    }
}
