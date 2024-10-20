
using System.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

namespace ZC_Informes.Services
{
    public class ReportSqlService : IReportSqlService
    {


        // =============== Variables o propiedades para almacenar los datos
        private readonly string connectionString;



        //  =============== Constructor
        public ReportSqlService()
        {
            // Obtén la cadena de conexión desde app.config
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        }



        //  =============== Metodo para realizar query a SQL y retornar la lista de las categorias de informe
        public async Task<IEnumerable<ReportSqlCategoryModel>> GetReportCategoryAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<ReportSqlCategoryModel>(sqlQuery);
                return result;
            }
        }



        //  =============== Metodo para realizar query a SQL y retornar la lista de los datos de los informes
        public async Task<IEnumerable<ReportSqlDataModel>> GetReportDataAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<ReportSqlDataModel>(sqlQuery);
                return result;
            }
        }



        //  =============== Metodo para realizar query a SQL y retornar la lista de las categorias de informe en formato para añadir a ListView
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
