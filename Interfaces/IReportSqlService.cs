using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    internal interface IReportSqlService
    {
        Task<IEnumerable<ReportSqlCategoryFormattedModel>> GetReportCategoryAsync(string sqlQuery);
        Task<IEnumerable<ReportSqlDataFormattedModel>> GetReportDataAsync(string sqlQuery, object parameters);
        Task<IEnumerable<ReportSqlReportListModel>> GetReportListAsync(string sqlQuery, object parameters);
    }
}
