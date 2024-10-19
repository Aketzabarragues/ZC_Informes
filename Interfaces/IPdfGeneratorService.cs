

using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IPdfGeneratorService
    {
        void GeneratePdf(string filePath, ReportConfigurationModel reportConfiguration);
    }
}
