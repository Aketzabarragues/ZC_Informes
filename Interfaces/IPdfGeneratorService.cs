using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IPdfGeneratorService
    {
        public void GeneratePdf(string filePath,
            ReportConfigFullModel reportConfiguration,
            IEnumerable<ReportSqlDataFormattedModel> table1HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table1DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table2HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table2DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table3HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table3DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table4HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table4DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table5HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table5DataSql);
    }
}
