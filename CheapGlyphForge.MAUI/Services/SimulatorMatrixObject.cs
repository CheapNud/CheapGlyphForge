// CheapGlyphForge.MAUI/Services/SimulatorMatrixObject.cs
using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simple implementation of IGlyphMatrixObject for simulator
/// </summary>
internal sealed record SimulatorMatrixObject(
    int PositionX,
    int PositionY,
    int Orientation = 0,
    int Scale = 100,
    int Brightness = 255,
    int Transparency = 0,
    string? Text = null,
    GlyphTextStyle TextStyle = GlyphTextStyle.Default,
    GlyphMarqueeType MarqueeType = GlyphMarqueeType.None) : IGlyphMatrixObject;