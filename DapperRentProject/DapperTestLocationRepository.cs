using Dapper;
using Microsoft.Data.SqlClient;
using RentProject.Shared.DTO;

namespace RentProject.Repository
{
    public class DapperTestLocationRepository
    {
        private readonly string _connectionString;

        public DapperTestLocationRepository(string connectionString)
        { 
            _connectionString = connectionString;
        }

        public List<TestLocationLookupRow> GetTestLocationLookup()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sql = @"
            SELECT 
                tl.TestLocationId,
                tl.TestLocationName,
                tl.TestAreaId,
                ta.TestAreaName
            FROM dbo.TestLocation tl
            LEFT JOIN dbo.TestArea ta ON ta.TestAreaId = tl.TestAreaId
            ORDER BY ta.TestAreaName, tl.TestLocationId ;";

            return connection.Query<TestLocationLookupRow>(sql).ToList();
        }
    }
}
