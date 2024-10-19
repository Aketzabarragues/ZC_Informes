
using System.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

namespace ZC_Informes.Services
{
    public class ReportSqlService : IReportSqlService
    {

        private readonly string connectionString;

        public ReportSqlService()
        {
            // Obtén la cadena de conexión desde app.config
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        }



        public async Task<IEnumerable<ReportSqlCategoryModel>> GetReportCategoryAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<ReportSqlCategoryModel>(sqlQuery);
                return result;
            }
        }



        public async Task<IEnumerable<ReportSqlDataModel>> GetReportDataAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<ReportSqlDataModel>(sqlQuery);
                return result;
            }
        }



        public async Task<IEnumerable<ReportSqlReportList>> GetReportListAsync(string sqlQuery, object parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<ReportSqlReportList> (sqlQuery, parameters);
                return result;
            }
        }


    }
}
