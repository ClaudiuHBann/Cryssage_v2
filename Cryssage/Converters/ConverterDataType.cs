﻿using System.Globalization;

namespace Cryssage.Converters.ConverterDataType
{
public class InverseBoolean : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !((bool)value);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
}
}
