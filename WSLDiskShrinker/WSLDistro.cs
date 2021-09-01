using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WSLDiskShrinker;

public class WSLDistro : INotifyPropertyChanged
{
	private long _size;
	public PackIconKind Icon { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }

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
			_ => PackIconKind.Linux
		};
	}
	public event PropertyChangedEventHandler? PropertyChanged;

	[NotifyPropertyChangedInvocator]
	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

