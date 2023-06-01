using System.Globalization;

namespace Cryssage.Converters.ConverterDataMessage
{
public class Time : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => ((DateTime)value).ToLocalTime().ToShortTimeString();

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => DateTime.Parse((string)value);
}

public class Path : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => System.IO.Path.GetFileName((string)value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("Could not convert narrowed file name to file path!");
}

public class Size : IValueConverter
{
    static readonly string[] measure = new[] { "B", "KB", "MB", "GB", "TB" };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        float size = (uint)value;
        byte measureindex = 0;

        while (size >= 1000f)
        {
            measureindex++;
            size /= 1000f;
        }

        return size.ToString("0.## ") + measure[measureindex];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("Could not convert narrowed file size to file size!");
}
}
