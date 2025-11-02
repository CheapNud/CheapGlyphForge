using CheapGlyphForge.Core.Interfaces;
using Com.Nothing.Ketchum;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

/// <summary>
/// Wrapper for the native GlyphFrame.Builder that implements our clean interface
/// </summary>
internal class GlyphFrameBuilderWrapper(GlyphFrame.Builder nativeBuilder) : IGlyphFrameBuilder
{
    private readonly GlyphFrame.Builder _nativeBuilder = nativeBuilder ?? throw new ArgumentNullException(nameof(nativeBuilder));

    public IGlyphFrameBuilder AddChannel(int channel)
    {
        _nativeBuilder.BuildChannel(channel);
        return this;
    }

    public IGlyphFrameBuilder AddChannels(params int[] channels)
    {
        foreach (var channel in channels)
            _nativeBuilder.BuildChannel(channel);
        return this;
    }

    public IGlyphFrameBuilder AddChannelA()
    {
        _nativeBuilder.BuildChannelA();
        return this;
    }

    public IGlyphFrameBuilder AddChannelB()
    {
        _nativeBuilder.BuildChannelB();
        return this;
    }

    public IGlyphFrameBuilder AddChannelC()
    {
        _nativeBuilder.BuildChannelC();
        return this;
    }

    public IGlyphFrameBuilder AddChannelD()
    {
        _nativeBuilder.BuildChannelD();
        return this;
    }

    public IGlyphFrameBuilder AddChannelE()
    {
        _nativeBuilder.BuildChannelE();
        return this;
    }

    public IGlyphFrameBuilder SetPeriod(int milliseconds)
    {
        _nativeBuilder.BuildPeriod(milliseconds);
        return this;
    }

    public IGlyphFrameBuilder SetCycles(int cycles)
    {
        _nativeBuilder.BuildCycles(cycles);
        return this;
    }

    public IGlyphFrameBuilder SetInterval(int milliseconds)
    {
        _nativeBuilder.BuildInterval(milliseconds);
        return this;
    }

    public IGlyphFrame Build()
    {
        var nativeFrame = _nativeBuilder.Build();
        if (nativeFrame == null)
            throw new InvalidOperationException("Failed to build native GlyphFrame");

        return new GlyphFrameWrapper(nativeFrame);
    }
}
