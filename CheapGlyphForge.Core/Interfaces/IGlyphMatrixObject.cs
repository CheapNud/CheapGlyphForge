
using CheapGlyphForge.Core.Models;

/// <summary>
/// Represents an object that can be rendered on the matrix
/// </summary>
public interface IGlyphMatrixObject
{
    int PositionX { get; }
    int PositionY { get; }
    int Orientation { get; }
    int Scale { get; }
    int Brightness { get; }
    int Transparency { get; }
    string? Text { get; }
    GlyphTextStyle TextStyle { get; }
    GlyphMarqueeType MarqueeType { get; }
}
