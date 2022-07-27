using System.Globalization;
using System.Windows.Data;

namespace WSLDiskShrinker.Common;

class FileSizeConverter : IValueConverter
{
    public static string Convert(long value)
    {
        double v = value; int i = 0;
        while (v >= 1023.995) { v /= 1024; i++; } // Max NTFS file size is 16 TiB…
        return $"{Math.Round(v, 2)} {"BKMGT"[i]}{(value < 1024 ? string.Empty : "iB")}";
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
            throw new NotSupportedException("Converting to non-string type is unsupported.");
        return Convert(value is long x ? x : 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
