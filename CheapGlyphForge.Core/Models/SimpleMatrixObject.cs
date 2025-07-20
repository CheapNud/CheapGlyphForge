// CheapGlyphForge.Core/Models/SimpleMatrixObject.cs
using CheapGlyphForge.Core.Interfaces;

namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Simple concrete implementation of IGlyphMatrixObject for use with simulators and testing
/// Can be used to create matrix objects programmatically
/// </summary>
public sealed record SimpleMatrixObject : IGlyphMatrixObject
{
    public int PositionX { get; init; }
    public int PositionY { get; init; }
    public int Orientation { get; init; } = 0;
    public int Scale { get; init; } = 100;
    public int Brightness { get; init; } = 255;
    public int Transparency { get; init; } = 0;
    public string? Text { get; init; }
    public GlyphTextStyle TextStyle { get; init; } = GlyphTextStyle.Default;
    public GlyphMarqueeType MarqueeType { get; init; } = GlyphMarqueeType.None;

    /// <summary>
    /// Create a simple text object at the specified position
    /// </summary>
    public static SimpleMatrixObject CreateText(string text, int x = 0, int y = 0, int brightness = 255, GlyphMarqueeType marquee = GlyphMarqueeType.None)
    {
        return new SimpleMatrixObject
        {
            Text = text,
            PositionX = x,
            PositionY = y,
            Brightness = brightness,
            MarqueeType = marquee
        };
    }

    /// <summary>
    /// Create a positioned object with custom properties
    /// </summary>
    public static SimpleMatrixObject Create(int x, int y, int orientation = 0, int scale = 100, int brightness = 255, int transparency = 0)
    {
        return new SimpleMatrixObject
        {
            PositionX = x,
            PositionY = y,
            Orientation = orientation,
            Scale = scale,
            Brightness = brightness,
            Transparency = transparency
        };
    }

    /// <summary>
    /// Create a text object with advanced styling
    /// </summary>
    public static SimpleMatrixObject CreateStyledText(
        string text,
        int x = 0,
        int y = 0,
        GlyphTextStyle style = GlyphTextStyle.Default,
        GlyphMarqueeType marquee = GlyphMarqueeType.None,
        int brightness = 255,
        int scale = 100)
    {
        return new SimpleMatrixObject
        {
            Text = text,
            PositionX = x,
            PositionY = y,
            TextStyle = style,
            MarqueeType = marquee,
            Brightness = brightness,
            Scale = scale
        };
    }
}