using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace WSLDiskShrinker
{ 
	public class WSLDistro: INotifyPropertyChanged
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

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
