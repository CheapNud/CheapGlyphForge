// CheapGlyphForge.Core/Interfaces/IGlyphMatrixFrame.cs
namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Represents a complete matrix frame with multiple layers
/// </summary>
public interface IGlyphMatrixFrame
{
    IGlyphMatrixObject? TopLayer { get; }
    IGlyphMatrixObject? MiddleLayer { get; }
    IGlyphMatrixObject? BottomLayer { get; }

    /// <summary>
    /// Render the frame to a flat 25x25 array
    /// </summary>
    int[] Render();

    /// <summary>
    /// Get the raw matrix data for this frame
    /// </summary>
    int[] GetRawData();
}