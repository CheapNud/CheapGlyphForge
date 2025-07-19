using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.Core.Helpers;

/// <summary>
/// Device detection helper
/// </summary>
public static class DeviceDetector
{
    public static GlyphDeviceType? CurrentDevice { get; private set; }

    public static void Initialize()
    {
        // [Inference] This would call the native Common.is20111() etc. methods
        // Implementation would check each device type and set CurrentDevice
    }
}