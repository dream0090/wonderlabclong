﻿using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Views.Converters;

public class ControlCenterOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var opacity = (double)value!;
        return 1 - opacity;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}