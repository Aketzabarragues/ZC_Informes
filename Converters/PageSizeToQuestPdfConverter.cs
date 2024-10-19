using System.Globalization;
using System.Windows.Data;
using QuestPDF.Helpers;


public class PageSizeToQuestPdfConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Tuple<string, bool> pageInfo)
        {
            string pageSize = pageInfo.Item1; // "A4", "A3", etc.
            bool isHorizontal = pageInfo.Item2; // true o false

            return GetPageSize(pageSize, isHorizontal);
        }

        throw new ArgumentException("Valor no valido para conversion");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No implementado
    }

    private PageSize GetPageSize(string pageSize, bool isHorizontal)
    {
        return pageSize switch
        {
            "A4" => isHorizontal ? PageSizes.A4.Landscape() : PageSizes.A4.Portrait(),
            "A3" => isHorizontal ? PageSizes.A3.Landscape() : PageSizes.A3.Portrait(),
            "Letter" => isHorizontal ? PageSizes.Letter.Landscape() : PageSizes.Letter.Portrait(),
            _ => throw new ArgumentOutOfRangeException($"Tamaño de pagina no sosportado: {pageSize}"),
        };
    }
}

