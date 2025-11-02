using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.Core.Helpers;

/// <summary>
/// Provides channel information for each device type, including advanced mode sub-zones
/// </summary>
public static class GlyphChannelInfoProvider
{
    public static List<GlyphChannelInfo> GetChannels(GlyphDeviceType device, bool advancedMode = false)
    {
        return device switch
        {
            GlyphDeviceType.Phone1 => advancedMode ? GetPhone1AdvancedChannels() : GetPhone1BasicChannels(),
            GlyphDeviceType.Phone2 => GetPhone2Channels(),
            GlyphDeviceType.Phone2a => GetPhone2aChannels(),
            GlyphDeviceType.Phone2aPlus => GetPhone2aPlusChannels(),
            GlyphDeviceType.Phone3 => GetPhone3Channels(),
            _ => []
        };
    }

    #region Phone (1) - Basic Mode (5 zones)

    private static List<GlyphChannelInfo> GetPhone1BasicChannels() =>
    [
        new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera",
            Description = "Camera ring glyph",
            Zones = [33]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Diagonal",
            Description = "Diagonal strip (8 LEDs)",
            Zones = [25, 26, 27, 28, 29, 30, 31, 32]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Battery",
            Description = "Horizontal battery bar (16 LEDs)",
            Zones = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]
        },
        new GlyphChannelInfo
        {
            Id = "D",
            Name = "USB Line",
            Description = "USB connection line (8 LEDs)",
            Zones = [17, 18, 19, 20, 21, 22, 23, 24]
        }
    ];

    #endregion

    #region Phone (1) - Advanced Mode (15 zones)

    private static List<GlyphChannelInfo> GetPhone1AdvancedChannels()
    {
        var channels = new List<GlyphChannelInfo>();

        // A: Camera (1 zone)
        channels.Add(new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera",
            Description = "Camera ring glyph",
            Zones = [33]
        });

        // B: Diagonal (split into 4 groups of 2 LEDs each)
        for (int i = 0; i < 4; i++)
        {
            var startZone = 25 + (i * 2);
            channels.Add(new GlyphChannelInfo
            {
                Id = $"B{i + 1}",
                Name = $"Diagonal {i + 1}",
                Description = $"Diagonal strip segment {i + 1}",
                Zones = [startZone, startZone + 1],
                ParentChannelId = "B"
            });
        }

        // C: Battery (split into 4 groups of 4 LEDs each)
        for (int i = 0; i < 4; i++)
        {
            var startZone = 1 + (i * 4);
            channels.Add(new GlyphChannelInfo
            {
                Id = $"C{i + 1}",
                Name = $"Battery {i + 1}",
                Description = $"Battery bar segment {i + 1}",
                Zones = [startZone, startZone + 1, startZone + 2, startZone + 3],
                ParentChannelId = "C"
            });
        }

        // D: USB Line (split into 4 groups of 2 LEDs each)
        for (int i = 0; i < 4; i++)
        {
            var startZone = 17 + (i * 2);
            channels.Add(new GlyphChannelInfo
            {
                Id = $"D{i + 1}",
                Name = $"USB Line {i + 1}",
                Description = $"USB line segment {i + 1}",
                Zones = [startZone, startZone + 1],
                ParentChannelId = "D"
            });
        }

        return channels;
    }

    #endregion

    #region Phone (2)

    private static List<GlyphChannelInfo> GetPhone2Channels() =>
    [
        new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera",
            Description = "Camera ring glyph",
            Zones = [1]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Diagonal",
            Description = "Diagonal line",
            Zones = [2]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Circle",
            Description = "Large center circle (24 LEDs)",
            Zones = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26]
        },
        new GlyphChannelInfo
        {
            Id = "D",
            Name = "USB Line",
            Description = "Vertical USB line (7 LEDs)",
            Zones = [27, 28, 29, 30, 31, 32, 33]
        },
        new GlyphChannelInfo
        {
            Id = "E",
            Name = "USB Dot",
            Description = "USB connection dot (6 LEDs)",
            Zones = [34, 35, 36, 37, 38, 39]
        }
    ];

    #endregion

    #region Phone (2a) and (2a) Plus

    private static List<GlyphChannelInfo> GetPhone2aChannels() =>
    [
        new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera",
            Description = "Camera outline",
            Zones = [25]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Vertical",
            Description = "Vertical lines",
            Zones = [24]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Battery",
            Description = "Battery indicator (24 LEDs)",
            Zones = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23]
        }
    ];

    private static List<GlyphChannelInfo> GetPhone2aPlusChannels() => GetPhone2aChannels();

    #endregion

    #region Phone (3)

    private static List<GlyphChannelInfo> GetPhone3Channels() =>
    [
        new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera Ring",
            Description = "Camera ring (11 LEDs)",
            Zones = [20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "USB Strip",
            Description = "USB connection strip (5 LEDs)",
            Zones = [31, 32, 33, 34, 35]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Bottom Bar",
            Description = "Bottom horizontal bar (20 LEDs)",
            Zones = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19]
        }
    ];

    #endregion

    public static string GetChannelTooltip(GlyphChannelInfo channel)
    {
        var ledCount = channel.Zones.Length;
        var ledText = ledCount == 1 ? "1 LED" : $"{ledCount} LEDs";
        return $"{channel.Name} - {channel.Description} ({ledText})";
    }
}
