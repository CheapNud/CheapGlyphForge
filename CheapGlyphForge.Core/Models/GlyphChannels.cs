using CheapGlyphForge.Core.Helpers;

namespace CheapGlyphForge.Core.Models;

/// <summary>
/// Static class containing channel constants for each device
/// </summary>
public static class GlyphChannels
{
    // Common channels - adjust based on device detection
    public static int[] A => GetChannelsForDevice(DeviceDetector.CurrentDevice, "A");
    public static int[] B => GetChannelsForDevice(DeviceDetector.CurrentDevice, "B");
    public static int[] C => GetChannelsForDevice(DeviceDetector.CurrentDevice, "C");
    public static int[] D => GetChannelsForDevice(DeviceDetector.CurrentDevice, "D");
    public static int[] E => GetChannelsForDevice(DeviceDetector.CurrentDevice, "E");

    // Device-specific channel mappings
    public static class Phone1
    {
        public static readonly int[] A = [33];
        public static readonly int[] B = [25, 26, 27, 28, 29, 30, 31, 32];
        public static readonly int[] C = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        public static readonly int[] D = [17, 18, 19, 20, 21, 22, 23, 24];
    }

    public static class Phone2
    {
        public static readonly int[] A = [1];
        public static readonly int[] B = [2];
        public static readonly int[] C = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26];
        public static readonly int[] D = [27, 28, 29, 30, 31, 32, 33];
        public static readonly int[] E = [34, 35, 36, 37, 38, 39];
    }

    public static class Phone2a
    {
        public static readonly int[] A = [25];
        public static readonly int[] B = [24];
        public static readonly int[] C = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
    }

    public static class Phone2aPlus
    {
        public static readonly int[] A = [25];
        public static readonly int[] B = [24];
        public static readonly int[] C = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
    }

    public static class Phone3
    {
        public static readonly int[] A = [20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30];
        public static readonly int[] B = [31, 32, 33, 34, 35];
        public static readonly int[] C = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19];
    }

    private static int[] GetChannelsForDevice(GlyphDeviceType? device, string channel) => device switch
    {
        GlyphDeviceType.Phone1 => channel switch
        {
            "A" => Phone1.A,
            "B" => Phone1.B,
            "C" => Phone1.C,
            "D" => Phone1.D,
            _ => []
        },
        GlyphDeviceType.Phone2 => channel switch
        {
            "A" => Phone2.A,
            "B" => Phone2.B,
            "C" => Phone2.C,
            "D" => Phone2.D,
            "E" => Phone2.E,
            _ => []
        },
        GlyphDeviceType.Phone2a => channel switch
        {
            "A" => Phone2a.A,
            "B" => Phone2a.B,
            "C" => Phone2a.C,
            _ => []
        },
        GlyphDeviceType.Phone2aPlus => channel switch
        {
            "A" => Phone2aPlus.A,
            "B" => Phone2aPlus.B,
            "C" => Phone2aPlus.C,
            _ => []
        },
        GlyphDeviceType.Phone3 => channel switch
        {
            "A" => Phone3.A,
            "B" => Phone3.B,
            "C" => Phone3.C,
            _ => []
        },
        _ => []
    };
}
