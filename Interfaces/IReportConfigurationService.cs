using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IReportConfigurationService
    {
        ReportConfigurationModel LoadConfiguration(string filePath);
    }
}

