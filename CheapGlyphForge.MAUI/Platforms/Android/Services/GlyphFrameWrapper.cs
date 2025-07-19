using CheapGlyphForge.Core.Interfaces;
using Com.Nothing.Ketchum;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

/// <summary>
/// Wrapper for the native GlyphFrame that implements our clean interface
/// </summary>
internal class GlyphFrameWrapper(GlyphFrame nativeFrame) : IGlyphFrame
{
    private readonly GlyphFrame _nativeFrame = nativeFrame ?? throw new ArgumentNullException(nameof(nativeFrame));

    public int[] Channels => _nativeFrame.GetChannel() ?? [];
    public int Period => _nativeFrame.Period;
    public int Cycles => _nativeFrame.Cycles;
    public int Interval => _nativeFrame.Interval;
}