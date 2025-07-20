// CheapGlyphForge.MAUI/Services/SimulatorGlyphFrameBuilder.cs
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphFrameBuilder for development
/// </summary>
internal sealed class SimulatorGlyphFrameBuilder : IGlyphFrameBuilder
{
    private readonly List<int> _channels = [];
    private int _period = 1000;
    private int _cycles = 1;
    private int _interval = 0;

    public IGlyphFrameBuilder AddChannel(int channel)
    {
        _channels.Add(channel);
        Debug.WriteLine($"SimulatorFrameBuilder: Added channel {channel}");
        return this;
    }

    public IGlyphFrameBuilder AddChannels(params int[] channels)
    {
        _channels.AddRange(channels);
        Debug.WriteLine($"SimulatorFrameBuilder: Added channels [{string.Join(", ", channels)}]");
        return this;
    }

    public IGlyphFrameBuilder AddChannelA()
    {
        _channels.AddRange(GlyphChannels.A);
        Debug.WriteLine("SimulatorFrameBuilder: Added Channel A");
        return this;
    }

    public IGlyphFrameBuilder AddChannelB()
    {
        _channels.AddRange(GlyphChannels.B);
        Debug.WriteLine("SimulatorFrameBuilder: Added Channel B");
        return this;
    }

    public IGlyphFrameBuilder AddChannelC()
    {
        _channels.AddRange(GlyphChannels.C);
        Debug.WriteLine("SimulatorFrameBuilder: Added Channel C");
        return this;
    }

    public IGlyphFrameBuilder AddChannelD()
    {
        _channels.AddRange(GlyphChannels.D);
        Debug.WriteLine("SimulatorFrameBuilder: Added Channel D");
        return this;
    }

    public IGlyphFrameBuilder AddChannelE()
    {
        _channels.AddRange(GlyphChannels.E);
        Debug.WriteLine("SimulatorFrameBuilder: Added Channel E");
        return this;
    }

    public IGlyphFrameBuilder SetPeriod(int milliseconds)
    {
        _period = milliseconds;
        Debug.WriteLine($"SimulatorFrameBuilder: Set period to {milliseconds}ms");
        return this;
    }

    public IGlyphFrameBuilder SetCycles(int cycles)
    {
        _cycles = cycles;
        Debug.WriteLine($"SimulatorFrameBuilder: Set cycles to {cycles}");
        return this;
    }

    public IGlyphFrameBuilder SetInterval(int milliseconds)
    {
        _interval = milliseconds;
        Debug.WriteLine($"SimulatorFrameBuilder: Set interval to {milliseconds}ms");
        return this;
    }

    public IGlyphFrame Build()
    {
        Debug.WriteLine($"SimulatorFrameBuilder: Building frame with {_channels.Count} channels");
        return new SimulatorGlyphFrame([.. _channels], _period, _cycles, _interval);
    }
}
