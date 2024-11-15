using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

public class ConfigBoolService : IConfigBoolService
{

    //  =============== Servicios inyectados
    private readonly ISnackbarService _snackbarService;



    //  =============== Constructor
    public ConfigBoolService()
    {
        _snackbarService = App.ServiceProvider!.GetRequiredService<ISnackbarService>();

    }


    //  =============== Metodo para cargar el archivo JSON
    public List<ConfigBoolModel> LoadConfigBool(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var configBool = JsonSerializer.Deserialize<List<ConfigBoolModel>>(json);
        return configBool ?? new List<ConfigBoolModel> { new ConfigBoolModel() };

    }


}
