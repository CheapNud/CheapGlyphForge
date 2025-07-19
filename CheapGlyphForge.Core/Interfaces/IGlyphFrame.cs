// CheapGlyphForge.Core/Interfaces/IGlyphFrameBuilder.cs
namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Represents a configured Glyph frame ready for execution
/// </summary>
public interface IGlyphFrame
{
    int[] Channels { get; }
    int Period { get; }
    int Cycles { get; }
    int Interval { get; }
}
