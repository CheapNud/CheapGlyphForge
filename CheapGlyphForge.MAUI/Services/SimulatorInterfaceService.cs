// CheapGlyphForge.MAUI/Services/SimulatorInterfaceService.cs
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using CheapGlyphForge.Core.Helpers;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphInterfaceService for cross-platform development
/// Provides same interface as AndroidInterfaceService but simulates behavior
/// </summary>
public sealed class SimulatorInterfaceService : IGlyphInterfaceService
{
    private const int SimulationDelayMs = 100;
    private const string ServiceName = "SimulatorInterfaceService";

    private bool _isConnected;
    private bool _isSessionOpen;
    private GlyphDeviceType? _registeredDevice;
    private readonly Dictionary<int, bool> _activeChannels = [];
    private readonly Random _random = new();

    #region Events
    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<GlyphErrorEventArgs>? ErrorOccurred;
    #endregion

    #region Properties
    public bool IsConnected => _isConnected;
    public GlyphDeviceType? RegisteredDevice => _registeredDevice;
    public bool IsSessionOpen => _isSessionOpen;
    #endregion

    #region Connection Management
    public async Task<bool> InitializeAsync()
    {
        Debug.WriteLine($"{ServiceName}: Initializing simulator...");

        // Simulate initialization delay
        await Task.Delay(SimulationDelayMs);

        _isConnected = true;
        ConnectionChanged?.Invoke(this, true);

        Debug.WriteLine($"{ServiceName}: Simulator initialized successfully");
        return true;
    }

    public async Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType)
    {
        Debug.WriteLine($"{ServiceName}: Registering device {deviceType}...");

        await Task.Delay(SimulationDelayMs);

        _registeredDevice = deviceType;
        Debug.WriteLine($"{ServiceName}: Device {deviceType} registered successfully");
        return true;
    }

    public async Task<bool> OpenSessionAsync()
    {
        if (!_isConnected)
        {
            Debug.WriteLine($"{ServiceName}: Cannot open session - not connected");
            return false;
        }

        Debug.WriteLine($"{ServiceName}: Opening session...");
        await Task.Delay(SimulationDelayMs);

        _isSessionOpen = true;
        Debug.WriteLine($"{ServiceName}: Session opened successfully");
        return true;
    }

    public async Task CloseSessionAsync()
    {
        Debug.WriteLine($"{ServiceName}: Closing session...");
        await Task.Delay(SimulationDelayMs);

        _isSessionOpen = false;
        _activeChannels.Clear();

        Debug.WriteLine($"{ServiceName}: Session closed successfully");
    }

    public async Task ShutdownAsync()
    {
        Debug.WriteLine($"{ServiceName}: Shutting down simulator...");

        if (_isSessionOpen)
        {
            await CloseSessionAsync();
        }

        _isConnected = false;
        _registeredDevice = null;
        ConnectionChanged?.Invoke(this, false);

        Debug.WriteLine($"{ServiceName}: Simulator shutdown completed");
    }
    #endregion

    #region Glyph Control Operations
    public async Task<bool> ToggleChannelsAsync(params int[] channels)
    {
        if (!await EnsureSessionReady()) return false;

        Debug.WriteLine($"{ServiceName}: Toggling channels [{string.Join(", ", channels)}]");

        await Task.Delay(SimulationDelayMs);

        foreach (var channel in channels)
        {
            _activeChannels[channel] = !_activeChannels.GetValueOrDefault(channel);
            Debug.WriteLine($"{ServiceName}: Channel {channel} -> {(_activeChannels[channel] ? "ON" : "OFF")}");
        }

        Debug.WriteLine($"{ServiceName}: Toggle completed successfully");
        return true;
    }

    public async Task<bool> AnimateChannelsAsync(int[] channels, int period = 1000, int cycles = 1, int interval = 0)
    {
        if (!await EnsureSessionReady()) return false;

        Debug.WriteLine($"{ServiceName}: Animating channels [{string.Join(", ", channels)}] - Period: {period}ms, Cycles: {cycles}, Interval: {interval}ms");

        await Task.Delay(SimulationDelayMs);

        // Simulate animation by toggling channels
        foreach (var channel in channels)
        {
            _activeChannels[channel] = true;
        }

        // Simulate the animation duration
        _ = Task.Run(async () =>
        {
            for (int cycle = 0; cycle < cycles; cycle++)
            {
                await Task.Delay(period);

                // Simulate breathing effect
                foreach (var channel in channels)
                {
                    _activeChannels[channel] = _random.NextDouble() > 0.5;
                }

                if (interval > 0 && cycle < cycles - 1)
                {
                    await Task.Delay(interval);
                }
            }

            // Turn off after animation
            foreach (var channel in channels)
            {
                _activeChannels[channel] = false;
            }
        });

        Debug.WriteLine($"{ServiceName}: Animation started successfully");
        return true;
    }

    public async Task<bool> DisplayProgressAsync(int[] channels, int progress, bool reverse = false)
    {
        if (!await EnsureSessionReady()) return false;

        Debug.WriteLine($"{ServiceName}: Displaying progress {progress}% (reverse: {reverse})");

        await Task.Delay(SimulationDelayMs);

        // Simulate progress display on specified channels
        foreach (var channel in channels)
        {
            _activeChannels[channel] = progress > 0;
        }

        Debug.WriteLine($"{ServiceName}: Progress display completed successfully");
        return true;
    }

    public async Task<bool> DisplayProgressAndToggleAsync(int[] channels, int progress, bool reverse = false)
    {
        if (!await EnsureSessionReady()) return false;

        Debug.WriteLine($"{ServiceName}: Displaying progress {progress}% with toggle (reverse: {reverse})");

        await Task.Delay(SimulationDelayMs);

        // Simulate progress and toggle behavior
        foreach (var channel in channels)
        {
            _activeChannels[channel] = progress > 50; // Simulate threshold behavior
        }

        Debug.WriteLine($"{ServiceName}: Progress and toggle completed successfully");
        return true;
    }

    public async Task TurnOffAsync()
    {
        if (!await EnsureSessionReady()) return;

        Debug.WriteLine($"{ServiceName}: Turning off all lights...");

        await Task.Delay(SimulationDelayMs);

        _activeChannels.Clear();

        Debug.WriteLine($"{ServiceName}: All lights turned off successfully");
    }
    #endregion

    #region Frame Builder Access
    public IGlyphFrameBuilder CreateFrameBuilder()
    {
        Debug.WriteLine($"{ServiceName}: Creating simulator frame builder");
        return new SimulatorGlyphFrameBuilder();
    }
    #endregion

    #region Helper Methods
    private async Task<bool> EnsureSessionReady()
    {
        if (!_isConnected)
        {
            Debug.WriteLine($"{ServiceName}: Service not connected");
            return false;
        }

        if (!_isSessionOpen)
        {
            Debug.WriteLine($"{ServiceName}: No active session - attempting to open...");
            return await OpenSessionAsync();
        }

        return true;
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Get current state of active channels (for debugging/testing)
    /// </summary>
    public Dictionary<int, bool> GetActiveChannels() => new(_activeChannels);

    /// <summary>
    /// Check if specific channel is currently active
    /// </summary>
    public bool IsChannelActive(int channel) => _activeChannels.GetValueOrDefault(channel);
    #endregion
}