using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HeavenUsurperToolkit
{
    public class WidthHeightToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement element)
            {
                return Math.Abs(element.ActualWidth - element.ActualHeight) < 0.001;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}