namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Represents a keyframe in a sequence with channel states at a specific time
/// </summary>
public class SequenceKeyframe
{
    public required double TimeInSeconds { get; init; }
    public required Dictionary<string, int> ChannelIntensities { get; init; }

    public SequenceKeyframe Clone()
    {
        return new SequenceKeyframe
        {
            TimeInSeconds = TimeInSeconds,
            ChannelIntensities = new Dictionary<string, int>(ChannelIntensities)
        };
    }
}
