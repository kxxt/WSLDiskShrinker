using System.Runtime.Serialization;

namespace WSLDiskShrinker.Common;

[Serializable]
public class CommandFailedException : Exception /* ISerializable */
{
    public Process? FailedProcess { get; }
    public CommandFailedException(Process p)
    {
        this.FailedProcess = p;
    }

    // Implement exception constructors (not used).
    public CommandFailedException() : base() { }
    public CommandFailedException(string? message) : base(message) { }
    public CommandFailedException(string? message, Exception? innerException) : base(message, innerException) { }

    // ISerializable should be implemented (not used).
    protected CommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
