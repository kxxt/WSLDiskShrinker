using System.Windows;
using Microsoft.Win32;

namespace WSLDiskShrinker;

static class Scanner
{
    private record ExactDistro(string Path, string Name);

    private static IEnumerable<ExactDistro> GetDistrosByRegistry()
    {
        var lxss = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Lxss");
        if (lxss is null)
        {
            MessageBox.Show("Can't open WSL's registry key! Did you enabled WSL on your computer?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
        var distroIDs = lxss!.GetSubKeyNames(); // '!' to silence warning about possible null reference...
        var distroKeys = from distroID in distroIDs
                         select lxss.OpenSubKey(distroID);
        return from distroKey in distroKeys
               let basePath = distroKey.GetValue("BasePath") as string
               where !string.IsNullOrWhiteSpace(basePath)
               let path = Path.Combine(basePath, "ext4.vhdx")
               where File.Exists(path)
               select new ExactDistro(
                   path,
                   distroKey.GetValue("DistributionName") as string ?? "<Unnamed>"
               );
    }

    public static IEnumerable<WSLDistro> Scan()
    {
        try
        {
            var distros = GetDistrosByRegistry();
            return from distro in distros
                   select new WSLDistro(distro.Path, distro.Name);
        }
        catch (Exception ex) when (ex is IOException
                                    or UnauthorizedAccessException
                                    or PathTooLongException)
        {
            MessageBox.Show("Error occurred while getting the size of the vhdx file.", "Error!",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Unknown error occurred! \r\nIf you think it's necessary, Please submit an issue in the github repo.\r\n{e.Message}\r\n{e.StackTrace}", "Fatal Error!",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.FailFast("Fatal Error", e);
        }
        return new List<WSLDistro>();
    }
}
