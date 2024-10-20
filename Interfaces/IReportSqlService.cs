using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    internal interface IReportSqlService
    {
        Task<IEnumerable<ReportSqlCategoryModel>> GetReportCategoryAsync(string sqlQuery);
        Task<IEnumerable<ReportSqlDataModel>> GetReportDataAsync(string sqlQuery);
        Task<IEnumerable<ReportSqlReportList>> GetReportListAsync(string sqlQuery, object parameters);
    }
}
