using System.Globalization;

using Cryssage.Views;
using Cryssage.Models;

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

public class Progress : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (float)value / 100f;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (float)value *
                                                                                                       100f;
}

public class UserLastMessageText : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var messages = (MessageModelView)value;
        if (messages.Items.Count == 0)
        {
            return "";
        }

        var messageLast = messages.Items.Last();
        return messageLast.Type switch {
            MessageType.TEXT => ((MessageTextModel)messageLast).Text,
            MessageType.FILE =>
                new Path().Convert(((MessageFileModel)messageLast).FilePath, targetType, parameter, culture),
            _ => "",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("Could not convert narrowed message info to message!");
}

public class UserLastMessageTime : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var messages = (MessageModelView)value;
        if (messages.Items.Count == 0)
        {
            return "";
        }

        var messageLast = messages.Items.Last();
        var messageLastTimestamp = messages.Items.Last().Timestamp;
        return new Time().Convert(messageLastTimestamp, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => new Time().ConvertBack((string)value, targetType, parameter,
                                                                             culture);
}
}
