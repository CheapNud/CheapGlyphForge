// CheapGlyphForge.Core/Interfaces/IGlyphInterfaceService.cs
using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Clean C# wrapper for Nothing Phone Glyph Interface SDK
/// Provides async operations for controlling individual Glyph lights
/// </summary>
public interface IGlyphInterfaceService
{
    #region Connection Management
    /// <summary>
    /// Initialize the Glyph service connection
    /// </summary>
    Task<bool> InitializeAsync();

    /// <summary>
    /// Register the app for a specific device type
    /// </summary>
    Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType);

    /// <summary>
    /// Open a session for controlling Glyph lights
    /// </summary>
    Task<bool> OpenSessionAsync();

    /// <summary>
    /// Close the current Glyph session
    /// </summary>
    Task CloseSessionAsync();

    /// <summary>
    /// Complete shutdown and cleanup
    /// </summary>
    Task ShutdownAsync();
    #endregion

    #region Glyph Control Operations
    /// <summary>
    /// Toggle specific channels on/off
    /// </summary>
    Task<bool> ToggleChannelsAsync(params int[] channels);

    /// <summary>
    /// Animate channels with breathing effect
    /// </summary>
    Task<bool> AnimateChannelsAsync(int[] channels, int period = 1000, int cycles = 1, int interval = 0);

    /// <summary>
    /// Display progress on C1/D1 channels (Phone-specific)
    /// </summary>
    Task<bool> DisplayProgressAsync(int[] channels, int progress, bool reverse = false);

    /// <summary>
    /// Display progress while toggling other channels
    /// </summary>
    Task<bool> DisplayProgressAndToggleAsync(int[] channels, int progress, bool reverse = false);

    /// <summary>
    /// Turn off all active Glyph lights
    /// </summary>
    Task TurnOffAsync();
    #endregion

    #region Quick Channel Access
    /// <summary>
    /// Quick access methods for common channel zones
    /// </summary>
    Task<bool> ToggleChannelAAsync() => ToggleChannelsAsync(GlyphChannels.A);
    Task<bool> ToggleChannelBAsync() => ToggleChannelsAsync(GlyphChannels.B);
    Task<bool> ToggleChannelCAsync() => ToggleChannelsAsync(GlyphChannels.C);
    Task<bool> ToggleChannelDAsync() => ToggleChannelsAsync(GlyphChannels.D);
    Task<bool> ToggleChannelEAsync() => ToggleChannelsAsync(GlyphChannels.E);

    Task<bool> AnimateChannelAAsync(int period = 1000, int cycles = 1) =>
        AnimateChannelsAsync(GlyphChannels.A, period, cycles);
    Task<bool> AnimateChannelBAsync(int period = 1000, int cycles = 1) =>
        AnimateChannelsAsync(GlyphChannels.B, period, cycles);
    Task<bool> AnimateChannelCAsync(int period = 1000, int cycles = 1) =>
        AnimateChannelsAsync(GlyphChannels.C, period, cycles);
    Task<bool> AnimateChannelDAsync(int period = 1000, int cycles = 1) =>
        AnimateChannelsAsync(GlyphChannels.D, period, cycles);
    Task<bool> AnimateChannelEAsync(int period = 1000, int cycles = 1) =>
        AnimateChannelsAsync(GlyphChannels.E, period, cycles);
    #endregion

    #region Frame Builder Access
    /// <summary>
    /// Create a new Glyph frame builder for complex operations
    /// </summary>
    IGlyphFrameBuilder CreateFrameBuilder();
    #endregion

    #region Events
    /// <summary>
    /// Fires when service connection state changes
    /// </summary>
    event EventHandler<bool> ConnectionChanged;

    /// <summary>
    /// Fires when service encounters an error
    /// </summary>
    event EventHandler<GlyphErrorEventArgs> ErrorOccurred;
    #endregion

    #region Properties
    /// <summary>
    /// Current connection status
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Currently registered device type
    /// </summary>
    GlyphDeviceType? RegisteredDevice { get; }

    /// <summary>
    /// Whether a session is currently open
    /// </summary>
    bool IsSessionOpen { get; }
    #endregion
}