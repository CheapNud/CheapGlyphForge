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
            Description = "Camera ring (glyphId 1)",
            Zones = [33]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Diagonal",
            Description = "Diagonal strip (glyphId 2)",
            Zones = [25, 26, 27, 28, 29, 30, 31, 32]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Battery",
            Description = "Horizontal battery bar (glyphId 3)",
            Zones = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]
        },
        new GlyphChannelInfo
        {
            Id = "D",
            Name = "USB Line",
            Description = "USB connection line (glyphId 4)",
            Zones = [17, 18, 19, 20, 21, 22, 23, 24]
        }
    ];

    #endregion

    #region Phone (1) - Advanced Mode (14 zones)

    private static List<GlyphChannelInfo> GetPhone1AdvancedChannels()
    {
        var channels = new List<GlyphChannelInfo>();

        // A: Camera (1 zone) - glyphId 1
        channels.Add(new GlyphChannelInfo
        {
            Id = "A",
            Name = "Camera",
            Description = "Camera ring (glyphId 1)",
            Zones = [33]
        });

        // B: Diagonal (1 zone, no sub-zones in official docs) - glyphId 2
        channels.Add(new GlyphChannelInfo
        {
            Id = "B",
            Name = "Diagonal",
            Description = "Diagonal strip (glyphId 2)",
            Zones = [25, 26, 27, 28, 29, 30, 31, 32]
        });

        // C: Battery (4 zones: 3.1-3.4) - glyphId 3
        var batteryNames = new[] { "Battery Top Right", "Battery Top Left", "Battery Bottom Left", "Battery Bottom Right" };
        for (int i = 0; i < 4; i++)
        {
            var startZone = 1 + (i * 4);
            channels.Add(new GlyphChannelInfo
            {
                Id = $"C{i + 1}",
                Name = batteryNames[i],
                Description = $"Battery zone 3.{i + 1}",
                Zones = [startZone, startZone + 1, startZone + 2, startZone + 3],
                ParentChannelId = "C"
            });
        }

        // D: USB Line (8 zones: 4.1-4.8, one LED each) - glyphId 4
        for (int i = 0; i < 8; i++)
        {
            var zone = 17 + i;
            channels.Add(new GlyphChannelInfo
            {
                Id = $"D{i + 1}",
                Name = $"USB Line {i + 1}",
                Description = $"USB line zone 4.{i + 1}",
                Zones = [zone],
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
            Name = "Camera Top",
            Description = "Camera top (glyphId 1)",
            Zones = [1]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Camera Bottom",
            Description = "Camera bottom (glyphId 2)",
            Zones = [2]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Diagonal + Battery",
            Description = "Diagonal line and battery circle (glyphIds 3-9)",
            Zones = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26]
        },
        new GlyphChannelInfo
        {
            Id = "D",
            Name = "USB Line",
            Description = "Vertical USB line (glyphId 10)",
            Zones = [27, 28, 29, 30, 31, 32, 33]
        },
        new GlyphChannelInfo
        {
            Id = "E",
            Name = "USB Dot",
            Description = "USB connection dot (glyphId 11)",
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
            Name = "Bottom Left",
            Description = "Bottom left glyph (glyphId 3)",
            Zones = [25]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Middle Right",
            Description = "Middle right glyph (glyphId 2)",
            Zones = [24]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Top Left",
            Description = "Top left glyph with 24 zones (glyphId 1)",
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
            Name = "Middle Right",
            Description = "Middle right glyph, 11 zones (glyphId 2 on 3a)",
            Zones = [20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30]
        },
        new GlyphChannelInfo
        {
            Id = "B",
            Name = "Bottom Left",
            Description = "Bottom left glyph, 5 zones (glyphId 3 on 3a)",
            Zones = [31, 32, 33, 34, 35]
        },
        new GlyphChannelInfo
        {
            Id = "C",
            Name = "Top Left",
            Description = "Top left glyph, 20 zones (glyphId 1 on 3a)",
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
