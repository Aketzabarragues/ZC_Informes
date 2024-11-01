using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IPdfGeneratorService
    {
        public void GeneratePdf(string filePath,
            ReportConfigurationModel reportConfiguration,
            IEnumerable<ReportSqlDataModelFormatted> table1HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table1DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table2HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table2DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table3HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table3DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table4HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table4DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table5HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table5DataSql);
    }
}
