using Dapper;
using Microsoft.Data.SqlClient;
using RentProject.Domain;
using RentProject.Shared.DTO;


namespace RentProject.Repository
{
    public class DapperRentTimeRepository
    {
        private readonly string _connectionString;

        public DapperRentTimeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // 連線測試(OK)
        public string TestConnection()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                connection.Open();

                int result = connection.ExecuteScalar<int>("SELECT 1;");

                return result == 1
                    ? "OK：連線成功，且可執行 SQL (SELECT 1 回傳1)"
                    : $"連線成功，但 SELECT 1 回傳非預期值：{result}";
            }
            catch (Exception ex)
            {
                return $"連線失敗：{ex.GetType().Name} - {ex.Message}";
            }
        }

        // 新增租時單
        public CreateRentTimeResult CreateRentTime(RentTime model)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            using var tx = connection.BeginTransaction();

            try
            {
                var insertSql = @"
                INSERT INTO dbo.RentTime
                (
                    BookingNo,
                    ProjectId,
                    TestLocationId, 
                    AssignedUserId, 
                    TestModeId, 
                    CreatedByUserId, 

                    TestInformation,
                    Notes,
                    ActualStartAt,
                    ActualEndAt,

                    HasLunch,
                    LunchMinutes,
                    HasDinner,
                    DinnerMinutes,

                    Status,
                    DeletedAt
                )
                OUTPUT INSERTED.RentTimeId
                VALUES
                (
                    @TempBookingNo,
                    @ProjectId,
                    @TestLocationId,
                    @AssignedUserId,
                    @TestModeId,
                    @CreatedByUserId,

                    @TestInformation,
                    @Notes,
                    @ActualStartAt,
                    @ActualEndAt,

                    @HasLunch,
                    @LunchMinutes,
                    @HasDinner,
                    @DinnerMinutes,

                    @Status,
                    NULL
                );";

                int rentTimeId = connection.ExecuteScalar<int>(insertSql, new
                {
                    TempBookingNo = "TMP",

                    ProjectId = 2,
                    TestLocationId = 2,
                    AssignedUserId = 2,
                    TestModeId = 2,
                    CreatedByUserId = 2,

                    model.TestInformation,
                    model.Notes,
                    model.ActualStartAt,
                    model.ActualEndAt,

                    model.HasLunch,
                    model.LunchMinutes,
                    model.HasDinner,

                    // DinnerMinutes 在資料庫是 NOT NULL 且沒有 default，所以一定要給值
                    DinnerMinutes = model.HasDinner ? model.DinnerMinutes : 0,

                    // enum 會以 int 送進去（Draft=0）
                    Status = (int)model.Status

                }, transaction: tx);

                string bookingNo = $"RF-{rentTimeId:D8}";

                var updateSql = @"UPDATE dbo.RentTime
                                SET BookingNo = @BookingNo
                                WHERE RentTimeId = @RentTimeId;";

                connection.Execute(updateSql, new
                {
                    BookingNo = bookingNo,
                    RentTimeId = rentTimeId
                }, transaction: tx);

                tx.Commit();

                return new CreateRentTimeResult
                {
                    RentTimeId = rentTimeId,
                    BookingNo = bookingNo,
                };
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        // 取得未刪除的所有案件
        public List<RentTimeListRow> GetActiveRentTimesForProjectView()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sql = @"
            SELECT
                rt.RentTimeId,
                rt.BookingNo, 

                -- 新表的 Id（之後下拉選單會用到）
                rt.ProjectId, 
                rt.TestLocationId,
                rt.AssignedUserId,
                rt.ActualStartAt AS StartTime,
                rt.ActualEndAt AS EndTime,
                
                -- 以下是「ProjectView 要顯示」的欄位：先用 JOIN 查出來
                p.ProjectNo,
                p.ProjectName,
                tl.TestLocationName AS Location,
                c.CustomerName AS CustomerName,
                ta.TestAreaName AS Area,
                pe.ProjectEngineerName AS PE
                FROM dbo.RentTime rt
                LEFT JOIN dbo.Project p ON p.ProjectId = rt.ProjectId
                LEFT JOIN dbo.TestLocation tl ON tl.TestLocationId = rt.TestLocationId
                LEFT JOIN dbo.Customer c ON c.CustomerId = p.CustomerId
                LEFT JOIN dbo.TestArea ta ON ta.TestAreaId = tl.TestAreaId
                LEFT JOIN dbo.ProjectEngineer pe ON pe.ProjectEngineerId = p.ProjectEngineerId
                WHERE rt.DeletedAt IS NULL
                ORDER BY CreatedAt DESC;";

            return connection.Query<RentTimeListRow>(sql).ToList();
        }

        // 透過案件編號查詢租時單
        public RentTime? GetRentTimeById(int rentTimeId)
        { 
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sql = @"
            SELECT
                rt.RentTimeId,
                rt.BookingNo,

                -- 新表欄位
                rt.ProjectId,
                rt.TestLocationId,
                rt.AssignedUserId,
                rt.TestModeId,
                rt.CreatedByUserId,
                rt.CreatedAt,
                rt.ModifiedByUserId,
                rt.ModifiedAt,
                rt.TestInformation,
                rt.Notes,
                rt.ActualStartAt,
                rt.ActualEndAt,
                rt.HasLunch,
                rt.LunchMinutes,
                rt.HasDinner,
                rt.DinnerMinutes,
                rt.Status,
                rt.DeletedAt,

                -- 下面是舊 UI/舊 Domain 會用到的欄位：用 JOIN 補回來
                p.ProjectNo,
                p.ProjectName,
                tl.TestLocationName AS Location,
                c.CustomerName,
                ta.TestAreaName AS Area,
                pe.ProjectEngineerName AS PE

            FROM dbo.RentTime rt
            LEFT JOIN dbo.Project p ON p.ProjectId = rt.ProjectId
            LEFT JOIN dbo.TestLocation tl ON tl.TestLocationId = rt.TestLocationId
            LEFT JOIN dbo.Customer c ON c.CustomerId = p.CustomerId
            LEFT JOIN dbo.TestArea ta ON ta.TestAreaId = tl.TestAreaId
            LEFT JOIN dbo.ProjectEngineer pe ON pe.ProjectEngineerId = p.ProjectEngineerId
            WHERE rt.RentTimeId = @RentTimeId
            AND rt.DeletedAt IS NULL;"; 

            return connection.QueryFirstOrDefault<RentTime>(sql, new { RentTimeId = rentTimeId });
        }

        // 更新租時單
        public int UpdateRentTime(RentTime model)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sql = @"
            UPDATE dbo.RentTime
            SET 
            ProjectId = @ProjectId,
            TestLocationId = @TestLocationId,
            AssignedUserId = @AssignedUserId,
            TestModeId = @TestModeId,

            TestInformation = @TestInformation,
            Notes = @Notes,
            ActualStartAt = @ActualStartAt,
            ActualEndAt = @ActualEndAt,

            HasLunch = @HasLunch,
            LunchMinutes = @LunchMinutes,
            HasDinner = @HasDinner,
            DinnerMinutes = @DinnerMinutes,

            Status = @Status,
            ModifiedByUserId = @ModifiedByUserId,
            ModifiedAt = @ModifiedAt

            WHERE RentTimeId = @RentTimeId
            AND DeletedAt IS NULL;";

            return connection.Execute(sql, new 
            {
                RentTimeId = 1,

                ProjectId = 2,
                TestLocationId = 2,
                AssignedUserId = 2,
                TestModeId = 2,

                model.TestInformation,
                model.Notes,
                model.ActualStartAt,
                model.ActualEndAt,

                model.HasLunch,
                model.LunchMinutes,
                model.HasDinner,
                DinnerMinutes = model.HasDinner ? model.DinnerMinutes : 0,

                Status = (int)model.Status,
                ModifiedByUserId = model.ModifiedByUserId ?? model.CreatedByUserId,
                ModifiedAt = DateTime.Now
            });
        }

        public int DeletedRentTime(int rentTimeId, string createdBy, DateTime modifiedDate)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sql = @"UPDATE dbo.RentTimes
                        SET IsDeleted = 1,
                            ModifiedBy = @ModifiedBy,
                            ModifiedDate = @ModifiedDate
                        WHERE RentTimeId = @RentTimeId;";

            return connection.Execute(sql, new { 
                RentTimeId = rentTimeId ,
                ModifiedBy = createdBy,
                ModifiedDate = modifiedDate
            });
        }
    }
}
