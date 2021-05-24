using System;
using System.Globalization;
using System.Windows.Data;

namespace WSLDiskShrinker.Common
{
	class FileSizeConverter : IValueConverter
	{
		public static string Convert(long value)
		{
			if (value < 500) return "0 bytes";
			var v = System.Convert.ToSingle(value);
			var i = -1;
			var byteUnits = new[] { "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };
			do
			{
				v /= 1024;
				i++;
			} while (v > 1024);
			return $"{Math.Round(Math.Max(v, 0.1), 2)} {byteUnits[i]}";
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(string))
				throw new NotSupportedException("Converting to non-string type is unsupported");
			if (value == null) return "0";
			return Convert((long) value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
