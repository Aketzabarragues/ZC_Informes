using ZC_Informes.Models;

namespace ZC_Informes.Interfaces
{
    public interface IConfigBoolService
    {
        List<ConfigBoolModel> LoadConfigBool(string filePath);
    }
}

