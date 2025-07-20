// CheapGlyphForge.MAUI/Services/SimulatorGlyphFrame.cs
using CheapGlyphForge.Core.Interfaces;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphFrame
/// </summary>
internal sealed record SimulatorGlyphFrame(
    int[] Channels,
    int Period,
    int Cycles,
    int Interval) : IGlyphFrame
{
    public int[] GetChannels() => Channels;
    public int GetPeriod() => Period;
    public int GetCycles() => Cycles;
    public int GetInterval() => Interval;
}