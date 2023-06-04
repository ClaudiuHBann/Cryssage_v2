using Cryssage.Models;

using System.Globalization;

namespace Cryssage.Converters.ConverterUIMessage
{
public class BackgroundColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => (bool)value ? MessageModel.RightColor : MessageModel.LeftColor;

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => (Color)value == MessageModel.RightColor;
}

public class LayoutOption : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => (bool)value ? MessageModel.RightLayoutOption
                                                              : MessageModel.LeftLayoutOption;

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => ((LayoutOptions)value).Alignment
                                                      == MessageModel.RightLayoutOption.Alignment;
}

public class Margin : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => (bool)value ? MessageModel.RightMargin : MessageModel.LeftMargin;

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => (Thickness)value == MessageModel.RightMargin;
}

public class OnlineStroke : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => (bool)value ? UserModel.ColorTransparent : UserModel.ColorStroke;

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => (Color)value == UserModel.ColorTransparent;
}

public class OnlineFill : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture) => (bool)value ? UserModel.ColorFill : UserModel.ColorTransparent;

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture) => (Color)value == UserModel.ColorFill;
}
}
