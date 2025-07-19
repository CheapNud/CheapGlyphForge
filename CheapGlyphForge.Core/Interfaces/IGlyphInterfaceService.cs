// CheapGlyphForge.Core/Interfaces/IGlyphInterfaceService.cs
using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.Core.Interfaces;

public interface IGlyphInterfaceService
{
    Task<bool> InitializeAsync();
    Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType);
    Task<bool> OpenSessionAsync();
    Task<bool> ToggleChannelsAsync(int[] channels);
    Task<bool> AnimateChannelsAsync(int[] channels, int period = 1000, int cycles = 1);
    Task TurnOffAsync();
    Task ShutdownAsync();

    event EventHandler<bool> ConnectionChanged;
}