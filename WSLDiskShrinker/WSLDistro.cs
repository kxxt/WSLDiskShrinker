using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;

namespace WSLDiskShrinker;

public class WSLDistro : INotifyPropertyChanged
{
    private long _size;
    public PackIconKind Icon { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

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

	public static PackIconKind GetIconByName(string name)
	{
		var lname = name.ToLowerInvariant();
		return lname switch
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
	}

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
