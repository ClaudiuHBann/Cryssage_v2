using System.Globalization;

namespace Cryssage.Converters.ConverterDataMessage
{
public class Time : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dateTime = (DateTime)value;
        return dateTime == DateTime.MinValue ? "" : ((DateTime)value).ToString("hh:mm tt");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new DateTime().ToString();
    }
}

public class Size : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var size = (int)value;

        var measure = new[] { "B", "KB", "MB", "GB", "TB" };
        int measureindex = 0, decimals = 0;

        while (size >= 1024)
        {
            measureindex++;
            decimals = size % 1000;
            size /= 1024;
        }

        return size.ToString() + (decimals > 0 ? "," + decimals.ToString() + " " : " ") + measure[measureindex];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return 0;
    }
}
}
