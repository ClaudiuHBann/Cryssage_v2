using Cryssage.Models;

using System.Globalization;

namespace Cryssage.Converters.ConverterUIMessage
{
    public class BackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? MessageModel.RightColor : MessageModel.LeftColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Color)value == MessageModel.RightColor;
        }
    }

    public class LayoutOption : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? MessageModel.RightLayoutOption : MessageModel.LeftLayoutOption;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((LayoutOptions)value).Alignment == MessageModel.RightLayoutOption.Alignment;
        }
    }

    public class Margin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? MessageModel.RightMargin : MessageModel.LeftMargin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Thickness)value == MessageModel.RightMargin;
        }
    }
}
