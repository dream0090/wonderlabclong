﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WonderLab.Views.Converters;

public class ReverseBoolValueConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return !(bool)value!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}