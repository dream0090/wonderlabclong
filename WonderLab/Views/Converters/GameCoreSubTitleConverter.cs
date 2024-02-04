﻿using Avalonia.Data.Converters;
using MinecraftLaunch.Classes.Models.Game;
using System;
using System.Globalization;
using System.Text;

namespace WonderLab.Views.Converters;

public class GameCoreSubTitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var core = value as GameEntry;

        if (core == null)
            return "Unknown";

        StringBuilder sb = new();
        if (!core.IsVanilla)
        {
            sb.Append($"{core.InheritsFrom?.Id} 依赖的加载器：").Append(core.MainLoaderType);
            return sb.ToString();
        }

        sb.Append($"原版 {core.Id}");
        return sb.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}