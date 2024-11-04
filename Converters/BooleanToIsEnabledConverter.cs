using System.Globalization;
using System.Windows.Data;



namespace ZC_Informes.Converters
{
    public class BooleanToIsEnabledConverter : IValueConverter
    {
        // Convierte un valor booleano a IsEnabled
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? true : false; // Devuelve true si es true, false si es false
            return false; // Valor por defecto, deshabilitado si no es un booleano
        }

        // No implementado
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Lanzar excepción si se intenta convertir de vuelta
        }
    }
}
