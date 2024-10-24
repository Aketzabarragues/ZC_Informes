using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IPdfGeneratorService
    {
        public void GeneratePdf(string filePath,
            ReportConfigurationModel reportConfiguration,
            IEnumerable<ReportSqlDataModel> tableGeneralHeaderDataSql,
            IEnumerable<ReportSqlDataModel> tableGeneralDataSql,
            IEnumerable<ReportSqlDataModel> tableAnaliticsHeaderDataSql,
            IEnumerable<ReportSqlDataModel> tableAnaliticsDataSql,
            IEnumerable<ReportSqlDataModel> tableProductionHeaderSql,
            IEnumerable<ReportSqlDataModel> tableProductionDataSql,
            IEnumerable<ReportSqlDataModel> TableDataHeaderSql,
            IEnumerable<ReportSqlDataModel> tableDataDataSql);
    }
}
