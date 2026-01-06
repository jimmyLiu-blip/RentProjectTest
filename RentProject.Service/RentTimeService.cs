using RentProject.Domain;
using RentProject.Repository;
using RentProject.Shared.DTO;

namespace RentProject.Service
{
    public class RentTimeService : IRentTimeService
    {
        private readonly IRentTimeRepository _repo;

        private static readonly TimeSpan LunchEnableAt = new(13, 0, 0);
        private static readonly TimeSpan DinnerEnableAt = new(18, 0, 0);

        public RentTimeService(IRentTimeRepository repo)
        { 
            _repo = repo;
        }

        // 連線測試
        public string TestConnection()
        { 
            return _repo.TestConnection();
        }

        // 新增租時單
        public CreateRentTimeResult CreateRentTime(RentTime model)
        {
            ValidateRequired(model);
            CalculateEstimated(model);

            return _repo.CreateRentTime(model);
        }

        // 取得案件清單
        public List<RentTimeListRow> GetProjectViewList()
        {
            return _repo.GetActiveRentTimesForProjectView();
        }

        // 透過編號取得租時單
        public RentTime GetRentTimeById(int rentTimeId)
        {
            var data = _repo.GetRentTimeById(rentTimeId);

            if (data == null)
            {
                throw new Exception($"找不到 RentTimeId={rentTimeId}");
            }

            return data;
        }

        // 透過編號更新租時單
        public void UpdateRentTimeById(RentTime model)
        {
            if (model.RentTimeId <= 0) throw new Exception("RentTimeId 不正確");

            // Update 前也要做：必填驗證 + 預估重算
            ValidateRequired(model);
            CalculateEstimated(model);

            model.ModifiedBy = model.CreatedBy;
            model.ModifiedDate = DateTime.Now;

            var rows = _repo.UpdateRentTime(model);
            if (rows != 1) throw new Exception($"更新失敗，受影響筆數={rows}");
        }

        // 刪除租時單
        public void DeletedRentTime(int rentTimeId, int modifiedByUserId)
        {
            if (rentTimeId <= 0) throw new Exception("RentTimeId 不正確");

            var rows = _repo.DeletedRentTime(rentTimeId, modifiedByUserId);

            if (rows != 1) throw new Exception($"刪除失敗，受影響筆數={rows}");
        }

        private static void ValidateRequired(RentTime model)
        {   
            if (model.ProjectId <= 0) throw new Exception("ProjectId 必填");
            if (model.TestLocationId <= 0) throw new Exception("TestLocationId 必填");

            if (!model.ActualStartAt.HasValue || !model.ActualEndAt.HasValue)
            { 
                model.HasLunch = false;
                model.LunchMinutes = 0;

                model.HasDinner = false;
                model.DinnerMinutes = 0;

                return;
            }

            var start = model.ActualStartAt.Value;
            var end = model.ActualEndAt.Value;

            if (start > end)
            {
                throw new Exception("結束時間不可早於開始時間");
            }

            bool canLunch = end.TimeOfDay >= LunchEnableAt && start.TimeOfDay < LunchEnableAt;
            bool canDinner = end.TimeOfDay >= DinnerEnableAt && start.TimeOfDay < DinnerEnableAt;

            if (!canLunch)
            {
                model.HasLunch = false;
                model.LunchMinutes = 0;
            }
            else
            {
                if (model.HasLunch) model.LunchMinutes = 60;
                else model.LunchMinutes = 0;
            }

            if (!canDinner)
            {
                model.HasDinner = false;
                model.DinnerMinutes = 0;
            }
            else
            {
                if (!model.HasDinner) model.DinnerMinutes = 0;
                else
                {
                    if (model.DinnerMinutes <= 0)
                        throw new Exception("HasDinner 勾選時，DinnerMinutes 必填且須 > 0");
                } 
            }
        }

        private static void CalculateEstimated(RentTime model)
        {
            if (!model.ActualStartAt.HasValue || !model.ActualEndAt.HasValue)
            { 
                model.EstimatedMinutes = 0;
                model.EstimatedHours = 0;
                return; //結束這個方法，不要在往下算
            }

            var start = model.ActualStartAt.Value;
            var end = model.ActualEndAt.Value;

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
