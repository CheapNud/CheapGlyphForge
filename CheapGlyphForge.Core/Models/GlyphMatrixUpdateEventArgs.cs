namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Event args for matrix update notifications
/// </summary>
public class GlyphMatrixUpdateEventArgs(int[] frameData) : EventArgs
{
    public int[] FrameData { get; } = frameData;
    public DateTime Timestamp { get; } = DateTime.Now;
    public int PixelCount => FrameData.Length;
}
