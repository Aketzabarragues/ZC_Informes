
namespace ZC_Informes.Models
{
    public class ConfigBoolModel
    {
        public int Id { get; set; } = 1;
        public string TextoTrue { get; set; } = "true";
        public string TextoFalse { get; set; } = "false";
        public bool ColorEnable { get; set; } = false;
        public string ColorTrue { get; set; } = string.Empty;
        public string ColorFalse { get; set; } = string.Empty;
    }
}
