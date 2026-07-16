namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Information about a glyph channel including its display name and zone mapping
/// </summary>
public class GlyphChannelInfo
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int[] Zones { get; init; }

    /// <summary>
    /// Parent channel ID if this is a sub-zone (for advanced mode)
    /// </summary>
    public string? ParentChannelId { get; init; }
}
