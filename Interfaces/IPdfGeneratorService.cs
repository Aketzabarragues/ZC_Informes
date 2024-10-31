using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IPdfGeneratorService
    {
        public void GeneratePdf(string filePath,
            ReportConfigurationModel reportConfiguration,
            IEnumerable<ReportSqlDataModel> table1HeaderSql,
            IEnumerable<ReportSqlDataModel> table1DataSql,
            IEnumerable<ReportSqlDataModel> table2HeaderSql,
            IEnumerable<ReportSqlDataModel> table2DataSql,
            IEnumerable<ReportSqlDataModel> table3HeaderSql,
            IEnumerable<ReportSqlDataModel> table3DataSql,
            IEnumerable<ReportSqlDataModel> table4HeaderSql,
            IEnumerable<ReportSqlDataModel> table4DataSql,
            IEnumerable<ReportSqlDataModel> table5HeaderSql,
            IEnumerable<ReportSqlDataModel> table5DataSql);
    }
}
