using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;

namespace WSLDiskShrinker;

public class WSLDistro : INotifyPropertyChanged
{
    public WSLDistro(string path, string name)
    {
        Path = path;
        Name = name;
        Icon = name == "Custom VHDX" ? PackIconKind.File : GetIconByName(name);
        Size = new FileInfo(path).Length;
    }

    public string Path { get; }
    public string Name { get; }

    public PackIconKind Icon { get; }
    static PackIconKind GetIconByName(string name) => name.ToLowerInvariant() switch
    {
        var x when x.Contains("ubuntu") => PackIconKind.Ubuntu,
        var x when x.Contains("mint") => PackIconKind.LinuxMint,
        var x when x.Contains("debian") => PackIconKind.Debian,
        var x when x.Contains("docker") => PackIconKind.Docker,
        var x when x.Contains("arch") => PackIconKind.Arch,
        var x when x.Contains("centos") => PackIconKind.Centos,
        var x when x.Contains("fedora") => PackIconKind.Fedora,
        _ => PackIconKind.Linux
    };

    private long _size;
    public long Size
    {
        get => _size;
        set
        {
            if (value == _size) return;
            _size = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
