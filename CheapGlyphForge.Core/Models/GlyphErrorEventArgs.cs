namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Event args for Glyph service errors
/// </summary>
public class GlyphErrorEventArgs(string message, Exception? exception = null) : EventArgs
{
    public string Message { get; } = message;
    public Exception? Exception { get; } = exception;
    public DateTime Timestamp { get; } = DateTime.Now;
}
