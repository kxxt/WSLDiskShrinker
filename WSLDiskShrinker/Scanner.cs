using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using Newtonsoft.Json;
using WSLDiskShrinker.Common;

namespace WSLDiskShrinker
{
	static class Scanner
	{
		public static void LoadDistroInfos()
		{
			distroInfos = JsonConvert.DeserializeObject<List<WSLDistroScanningInfo>>(File.ReadAllText("distros.json"));
		}
		private static List<WSLDistroScanningInfo> distroInfos;
		public static IEnumerable<string> GetDistroDiskPathByPackageName(string keyword)
		{
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			dir = Path.Combine(dir, "Packages");
			var packages = new DirectoryInfo(dir).EnumerateDirectories();
			var result = new List<string>();
			foreach (var package in packages)
			{
				if (package.Name.ToLower().Contains(keyword))
				{
					var file = Path.Combine(package.FullName, "LocalState", "ext4.vhdx");
					if (File.Exists(file)) result.Add(file);
				}
			}
			return result;
		}
		public static IEnumerable<string> GetWSLDistros()
		{
			var procs = new ProcessStartInfo("cmd.exe", "/c wsl -l -v")
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.Unicode
			};
			var proc = new Process { StartInfo = procs };
			proc.Start();
			var result = new List<string>();
			bool first = true;
			while (!proc.StandardOutput.EndOfStream)
			{
				var line = proc.StandardOutput.ReadLine();
				if (first)
				{
					first = false;
					continue;
				}

				if (!string.IsNullOrWhiteSpace(line))
				{
					var splits = line.Split();
					result.Add(splits[0] == "*" ? splits[1] : splits[2]);
				}
			}
			proc.WaitForExit();
			if (proc.ExitCode != 0) throw new CommandFailedException(proc);
			return result;
		}
		public static IEnumerable<WSLDistro> Scan()
		{
			try
			{
				LoadDistroInfos();
			}
			catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
			{
				MessageBox.Show("Error occurred when reading distro.json", "Error!",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return new List<WSLDistro>();
			}
			catch (JsonSerializationException)
			{
				MessageBox.Show("Bad format json: distro.json", "Error!",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return new List<WSLDistro>();
			}
			catch (Exception e)
			{
				MessageBox.Show("Unknown error occurred! \r\nIf you think it's necessary, Please submit an issue in the github repo.", "Fatal Error!",
					MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.FailFast("Fatal Error", e);
			}
			var distros = GetWSLDistros();
			var result = new List<WSLDistro>();
			var variables = new PathVariables();
			foreach (var distro in distros)
			{
				foreach (var info in distroInfos)
					if (distro == info.Identifier)
					{
						var r = new List<string>();
						if (info.Keywords != null) r.AddRange(
								from x in info.Keywords
								from path in GetDistroDiskPathByPackageName(x)
								select path
							);
						if (info.Paths != null) r.AddRange(
								from path in info.Paths
								where File.Exists(path.Inject(variables))
								select path.Inject(variables)
							);
						result.AddRange(
							from path in r.Distinct()
							select new WSLDistro
							{
								Icon = info.Icon,
								Name = distro,
								Path = path,
								Size = new FileInfo(path).Length
							}
						);

						break;
					}
			}

			return result;
		}
	}
}
