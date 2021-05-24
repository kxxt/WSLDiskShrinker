using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSLDiskShrinker.Common
{
	class BooleanToInverseVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool v = (bool) value;
			return v ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
