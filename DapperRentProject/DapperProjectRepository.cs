using Dapper;
using Microsoft.Data.SqlClient;
using RentProject.Shared.DTO;
using RentProject.UIModels;

namespace RentProject.Repository
{
    public class DapperProjectRepository:IProjectRepository
    {
        private readonly string _connectionString;

        public DapperProjectRepository(string connectionString)
        { 
            _connectionString = connectionString;
        }

        public List<ProjectLookupRow> GetProjectLookup()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var selectSql = @"
            SELECT 
                p.ProjectId,
                p.ProjectNo, 
                p.ProjectName,
                pe.ProjectEngineerName
            FROM dbo.Project p
            LEFT JOIN ProjectEngineer pe ON pe.ProjectEngineerId = p.ProjectEngineerId
            WHERE p.DeletedAt IS NULL
            ORDER BY p.ProjectNo;";

            return connection.Query<ProjectLookupRow>(selectSql).ToList();
        }
    }
}
