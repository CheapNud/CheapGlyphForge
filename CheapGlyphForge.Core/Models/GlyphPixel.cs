namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Represents a single pixel in the matrix
/// </summary>
public readonly record struct GlyphPixel(int X, int Y, int Intensity)
{
    public bool IsValid => X >= 0 && X < 25 && Y >= 0 && Y < 25 && Intensity >= 0;
}
