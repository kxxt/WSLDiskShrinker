namespace WSLDiskShrinker.Common;

public class CommandFailedException : Exception
{
	public Process FailedProcess { get; set; }
	public CommandFailedException(Process p)
	{
		this.FailedProcess = p;
	}
}

