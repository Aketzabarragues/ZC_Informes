// Importaciones necesarias
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



        // Constructor que obtiene la cadena de conexión desde app.config
        public ReportSqlService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        }



        public async Task<IEnumerable<ReportSqlCategoryModel>> GetReportCategoryAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ReportSqlCategoryModel>(sqlQuery);
            }
        }



        public async Task<IEnumerable<ReportSqlDataModelFormatted>> GetReportDataAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();             

                var result = await connection.QueryAsync<ReportSqlDataModel>(sqlQuery);

                // Formatear las fechas y horas en el resultado
                var formattedResult = result.Select(data => new ReportSqlDataModelFormatted
                {
                    Id = data.Id,
                    Id_Categoria = data.Id_Categoria,
                    Codigo = data.Codigo,
                    
                    Fecha_1_Date = data.Fecha_1.Date,
                    Fecha_2_Date = data.Fecha_2.Date,
                    Fecha_3_Date = data.Fecha_3.Date,
                    Fecha_4_Date = data.Fecha_4.Date,
                    
                    Hora_1_Time = data.Hora_1,
                    Hora_2_Time = data.Hora_2,
                    Hora_3_Time = data.Hora_3,
                    Hora_4_Time = data.Hora_4,
                    
                    String_1 = data.String_1,
                    String_2 = data.String_2,
                    String_3 = data.String_3,
                    String_4 = data.String_4,
                    String_5 = data.String_5,
                    String_6 = data.String_6,
                    String_7 = data.String_7,
                    String_8 = data.String_8,
                    String_9 = data.String_9,
                    String_10 = data.String_10,

                    Bool_1_Value = data.Bool_1,
                    Bool_2_Value = data.Bool_2,
                    Bool_3_Value = data.Bool_3,
                    Bool_4_Value = data.Bool_4,
                    Bool_5_Value = data.Bool_5,
                    Bool_6_Value = data.Bool_6,
                    Bool_7_Value = data.Bool_7,
                    Bool_8_Value = data.Bool_8,
                    Bool_9_Value = data.Bool_9,
                    Bool_10_Value = data.Bool_10,
                    Bool_11_Value = data.Bool_11,
                    Bool_12_Value = data.Bool_12,
                    Bool_13_Value = data.Bool_13,
                    Bool_14_Value = data.Bool_14,
                    Bool_15_Value = data.Bool_15,
                    Bool_16_Value = data.Bool_16,
                    Bool_17_Value = data.Bool_17,
                    Bool_18_Value = data.Bool_18,
                    Bool_19_Value = data.Bool_19,
                    Bool_20_Value = data.Bool_20,
                    Bool_21_Value = data.Bool_21,
                    Bool_22_Value = data.Bool_22,
                    Bool_23_Value = data.Bool_23,
                    Bool_24_Value = data.Bool_24,
                    Bool_25_Value = data.Bool_25,
                    Bool_26_Value = data.Bool_26,
                    Bool_27_Value = data.Bool_27,
                    Bool_28_Value = data.Bool_28,
                    Bool_29_Value = data.Bool_29,
                    Bool_30_Value = data.Bool_30,
                    Bool_31_Value = data.Bool_31,
                    Bool_32_Value = data.Bool_32,

                    Int_1_Value = data.Int_1,
                    Int_2_Value = data.Int_2,
                    Int_3_Value = data.Int_3,
                    Int_4_Value = data.Int_4,
                    Int_5_Value = data.Int_5,
                    Int_6_Value = data.Int_6,
                    Int_7_Value = data.Int_7,
                    Int_8_Value = data.Int_8,
                    Int_9_Value = data.Int_9,
                    Int_10_Value = data.Int_10,
                    Int_11_Value = data.Int_11,
                    Int_12_Value = data.Int_12,
                    Int_13_Value = data.Int_13,
                    Int_14_Value = data.Int_14,
                    Int_15_Value = data.Int_15,
                    Int_16_Value = data.Int_16,
                    Int_17_Value = data.Int_17,
                    Int_18_Value = data.Int_18,
                    Int_19_Value = data.Int_19,
                    Int_20_Value = data.Int_20,

                    Real_1_Value = data.Real_1,
                    Real_2_Value = data.Real_2,
                    Real_3_Value = data.Real_3,
                    Real_4_Value = data.Real_4,
                    Real_5_Value = data.Real_5,
                    Real_6_Value = data.Real_6,
                    Real_7_Value = data.Real_7,
                    Real_8_Value = data.Real_8,
                    Real_9_Value = data.Real_9,
                    Real_10_Value = data.Real_10,
                    Real_11_Value = data.Real_11,
                    Real_12_Value = data.Real_12,
                    Real_13_Value = data.Real_13,
                    Real_14_Value = data.Real_14,
                    Real_15_Value = data.Real_15,
                    Real_16_Value = data.Real_16,
                    Real_17_Value = data.Real_17,
                    Real_18_Value = data.Real_18,
                    Real_19_Value = data.Real_19,
                    Real_20_Value = data.Real_20,

                });

                return formattedResult;

            }
        }



        public async Task<IEnumerable<ReportSqlReportList>> GetReportListAsync(string sqlQuery, object parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ReportSqlReportList>(sqlQuery, parameters);
            }
        }

		
		
    }
	
	
	
}
