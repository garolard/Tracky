﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Tracky.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool) value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility) value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }
}