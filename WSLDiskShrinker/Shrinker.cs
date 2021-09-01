namespace WSLDiskShrinker;

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
		var tmp_file_info = new FileInfo(script_path)
		{
			Attributes = FileAttributes.Temporary
		};
#if NET5_0_OR_GREATER
		await File.WriteAllTextAsync(script_path, GetDiskpartScript(file));
#else
			await Task.Run(()=>File.WriteAllText(script_path, GetDiskpartScript(file)));
#endif
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
#if NET5_0_OR_GREATER
		await proc.WaitForExitAsync();
#else
			await proc.CustomWaitForExitAsync();
#endif

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

