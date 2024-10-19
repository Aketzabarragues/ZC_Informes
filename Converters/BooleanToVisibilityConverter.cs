using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace ZC_Informes.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        

        //  Convierte un valor booleano a Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Hidden : Visibility.Visible; //   Devuelve Hidden si es true, Visible si es false
            return Visibility.Visible; //   Valor por defecto
        }

        // No implementado
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Lanzar excepción si se intenta convertir de vuelta
        }

    }
}
