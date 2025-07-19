// CheapGlyphForge.Core/Models/GlyphMatrixUpdateEventArgs.cs
namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Event args for matrix update notifications
/// </summary>
public class GlyphMatrixUpdateEventArgs : EventArgs
{
    public string Description { get; }
    public int ElementCount { get; }
    public DateTime Timestamp { get; }
    public int[]? FrameData { get; }

    /// <summary>
    /// Create matrix update event with description and count
    /// </summary>
    public GlyphMatrixUpdateEventArgs(string description, int elementCount)
    {
        Description = description;
        ElementCount = elementCount;
        Timestamp = DateTime.Now;
        FrameData = null;
    }

    /// <summary>
    /// Create matrix update event with full frame data
    /// </summary>
    public GlyphMatrixUpdateEventArgs(int[] frameData)
    {
        Description = "Matrix frame updated";
        ElementCount = frameData.Length;
        Timestamp = DateTime.Now;
        FrameData = frameData;
    }

    /// <summary>
    /// Create matrix update event with description and frame data
    /// </summary>
    public GlyphMatrixUpdateEventArgs(string description, int[] frameData)
    {
        Description = description;
        ElementCount = frameData.Length;
        Timestamp = DateTime.Now;
        FrameData = frameData;
    }

    public int PixelCount => ElementCount;
}