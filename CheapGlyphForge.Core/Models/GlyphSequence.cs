namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Represents a timed sequence of glyph/matrix states
/// </summary>
public class GlyphSequence
{
    public string Name { get; set; } = "Untitled Sequence";
    public double DurationInSeconds { get; set; } = 5.0;
    public List<SequenceKeyframe> Keyframes { get; set; } = [];
    public bool Loop { get; set; }

    public void AddKeyframe(double timeInSeconds, Dictionary<string, int> channelStates)
    {
        // Remove existing keyframe at same time
        Keyframes.RemoveAll(k => Math.Abs(k.TimeInSeconds - timeInSeconds) < 0.01);

        // Add new keyframe and sort
        Keyframes.Add(new SequenceKeyframe
        {
            TimeInSeconds = timeInSeconds,
            ChannelIntensities = new Dictionary<string, int>(channelStates)
        });

        Keyframes = [.. Keyframes.OrderBy(k => k.TimeInSeconds)];
    }

    public void RemoveKeyframe(double timeInSeconds)
    {
        Keyframes.RemoveAll(k => Math.Abs(k.TimeInSeconds - timeInSeconds) < 0.01);
    }

    public Dictionary<string, int>? GetStateAtTime(double timeInSeconds)
    {
        if (Keyframes.Count == 0) return null;

        // Find surrounding keyframes
        var before = Keyframes.LastOrDefault(k => k.TimeInSeconds <= timeInSeconds);
        var after = Keyframes.FirstOrDefault(k => k.TimeInSeconds > timeInSeconds);

        if (before == null) return null;
        if (after == null) return before.ChannelIntensities;

        // Linear interpolation between keyframes
        var progress = (timeInSeconds - before.TimeInSeconds) / (after.TimeInSeconds - before.TimeInSeconds);
        var interpolated = new Dictionary<string, int>();

        foreach (var channel in before.ChannelIntensities.Keys.Union(after.ChannelIntensities.Keys))
        {
            var beforeValue = before.ChannelIntensities.GetValueOrDefault(channel, 0);
            var afterValue = after.ChannelIntensities.GetValueOrDefault(channel, 0);
            interpolated[channel] = (int)(beforeValue + (afterValue - beforeValue) * progress);
        }

        return interpolated;
    }
}
