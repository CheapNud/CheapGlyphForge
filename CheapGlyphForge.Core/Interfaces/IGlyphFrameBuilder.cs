// CheapGlyphForge.Core/Interfaces/IGlyphFrameBuilder.cs
namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Builder pattern for creating Glyph frames with multiple channels and timing
/// </summary>
public interface IGlyphFrameBuilder
{
    IGlyphFrameBuilder AddChannel(int channel);
    IGlyphFrameBuilder AddChannels(params int[] channels);
    IGlyphFrameBuilder AddChannelA();
    IGlyphFrameBuilder AddChannelB();
    IGlyphFrameBuilder AddChannelC();
    IGlyphFrameBuilder AddChannelD();
    IGlyphFrameBuilder AddChannelE();

    IGlyphFrameBuilder SetPeriod(int milliseconds);
    IGlyphFrameBuilder SetCycles(int cycles);
    IGlyphFrameBuilder SetInterval(int milliseconds);

    IGlyphFrame Build();
}
