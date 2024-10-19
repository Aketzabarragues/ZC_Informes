using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IReportConfigurationService
    {
        ReportConfigurationModel LoadConfiguration();
    }
}

