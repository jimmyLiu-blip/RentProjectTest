using Microsoft.Data.SqlClient;
using RentProject.Domain;
using Dapper;


namespace RentProject.Repository
{
    public class DapperRentTimeRepository
    {
        private readonly string _connectionString;

        public DapperRentTimeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

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

        public CreateRentTimeResult CreateRentTime(RentTime model)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            using var tx = connection.BeginTransaction();

            try
            {
                var insertSql = @"
                INSERT INTO dbo.RentTimes
                (
                    BookingNo,CreatedBy, Area, CustomerName, Sales, ProjectNo, ProjectName, PE, Location,
                    ContactName, Phone, TestInformation, EngineerName, SampleModel, SampleNo,
                    TestMode, TestItem, Notes, 
                    StartDate, EndDate, StartTime, EndTime, EstimatedMinutes, EstimatedHours,
                    HasLunch, LunchMinutes, HasDinner, DinnerMinutes
                )
                OUTPUT INSERTED.RentTimeId
                VALUES
                (
                    NULL, @CreatedBy, @Area, @CustomerName, @Sales, @ProjectNo, @ProjectName, @PE, @Location,
                    @ContactName, @Phone, @TestInformation, @EngineerName, @SampleModel, @SampleNo,
                    @TestMode, @TestItem, @Notes, 
                    @StartDate, @EndDate, @StartTime, @EndTime, @EstimatedMinutes, @EstimatedHours,
                    @HasLunch, @LunchMinutes, @HasDinner, @DinnerMinutes
                );";

                int rentTimeId = connection.ExecuteScalar<int>(insertSql, new 
                {
                    model.CreatedBy,
                    model.Area,
                    model.CustomerName,
                    model.Sales,
                    model.ProjectNo,
                    model.ProjectName,
                    model.PE,
                    model.Location,

                    model.ContactName,
                    model.Phone,
                    model.TestInformation,
                    model.EngineerName,
                    model.SampleModel,
                    model.SampleNo,
                    model.TestMode,
                    model.TestItem,
                    model.Notes,

                    model.StartDate,
                    model.EndDate,
                    model.StartTime,
                    model.EndTime,
                    model.EstimatedMinutes,
                    model.EstimatedHours,

                    model.HasLunch,
                    model.LunchMinutes,
                    model.HasDinner,
                    model.DinnerMinutes,

                }, transaction:tx);

                string bookingNo = $"RF-{rentTimeId:D8}";

                var updateSql = @"UPDATE dbo.RentTimes 
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
    }
}
