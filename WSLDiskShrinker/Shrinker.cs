using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WSLDiskShrinker.Common;

namespace WSLDiskShrinker
{
	static class Shrinker
	{
		public static string GetDiskpartScript(string file)
		{
			return $"select vdisk file=\"{file}\"\n" +
				   "compact vdisk\n" +
				   "exit\n";
		}

		public static async Task<FileInfo> GenerateDiskpartScript(string file)
		{
			var script_path = Path.GetTempFileName();
			var tmp_file_info = new FileInfo(script_path);
			tmp_file_info.Attributes = FileAttributes.Temporary;
			await File.WriteAllTextAsync(script_path, GetDiskpartScript(file));
			return tmp_file_info;
		}
		public static async Task ShrinkUsingDiskpart(FileInfo scriptFile)
		{
			var pinfo = new ProcessStartInfo
			{
				FileName = "diskpart",
				Arguments = $"/s {scriptFile.FullName}",
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true
			};
			var proc = new Process { StartInfo = pinfo };
			proc.Start();
			await proc.WaitForExitAsync();
			if (proc.ExitCode != 0) throw new CommandFailedException(proc);
			//catch (IOException)
			//{
			//	MessageBox.Show("I/O Error occurred while writing temp diskpart script file.", "Error!",
			//		MessageBoxButton.OK, MessageBoxImage.Error);
			//}
			//catch(Exception ex)
			//{
			//	MessageBox.Show("Unknown error occurred! \r\nIf you think it's necessary, Please submit an issue in the github repo.", "Fatal Error!",
			//		MessageBoxButton.OK, MessageBoxImage.Error);
			//	Environment.FailFast("Fatal Error", ex);
			//}
		}
	}
}
