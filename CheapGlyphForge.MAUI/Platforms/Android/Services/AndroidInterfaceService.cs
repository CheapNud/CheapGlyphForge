// CheapGlyphForge.MAUI/Platforms/Android/Services/AndroidInterfaceService.cs
using Android.Content;
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using CheapGlyphForge.Core.Helpers;
using Com.Nothing.Ketchum;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

/// <summary>
/// Android implementation of IGlyphInterfaceService using Nothing Phone Glyph SDK
/// </summary>
public class AndroidInterfaceService : IGlyphInterfaceService, IDisposable
{
    private readonly Context _context;
    private GlyphManager? _glyphManager;
    private GlyphManagerCallback? _callback;
    private bool _disposed;
    private TaskCompletionSource<bool>? _connectionTcs;

    public AndroidInterfaceService()
    {
        _context = Platform.CurrentActivity?.ApplicationContext ??
                   throw new InvalidOperationException("Android context not available");
    }

    #region Events
    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<GlyphErrorEventArgs>? ErrorOccurred;
    #endregion

    #region Properties
    public bool IsConnected { get; private set; }
    public GlyphDeviceType? RegisteredDevice { get; private set; }
    public bool IsSessionOpen { get; private set; }
    #endregion

    #region Connection Management

    public async Task<bool> InitializeAsync()
    {
        try
        {
            if (_glyphManager != null)
            {
                Debug.WriteLine("AndroidInterfaceService: Already initialized");
                return true;
            }

            Debug.WriteLine("AndroidInterfaceService: Starting initialization...");

            _glyphManager = GlyphManager.GetInstance(_context);
            _callback = new GlyphManagerCallback(this);
            _connectionTcs = new TaskCompletionSource<bool>();

            // Start the async initialization
            await Task.Run(() => _glyphManager.Init(_callback));

            // Wait for connection callback with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
                var connected = await _connectionTcs.Task.WaitAsync(cts.Token);
                Debug.WriteLine($"AndroidInterfaceService: Initialization completed - Connected: {connected}");
                return connected;
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("AndroidInterfaceService: Initialization timed out");
                ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Initialization timed out"));
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Initialization failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Initialization failed", ex));
            return false;
        }
    }

    public async Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType)
    {
        try
        {
            if (_glyphManager == null)
            {
                Debug.WriteLine("AndroidInterfaceService: Cannot register - service not initialized");
                return false;
            }

            Debug.WriteLine($"AndroidInterfaceService: Registering device {deviceType}...");

            await Task.Run(() =>
            {
                var deviceString = deviceType switch
                {
                    GlyphDeviceType.Phone1 => Glyph.Device20111,
                    GlyphDeviceType.Phone2 => Glyph.Device22111,
                    GlyphDeviceType.Phone2a => Glyph.Device23111,
                    GlyphDeviceType.Phone2aPlus => Glyph.Device23113,
                    GlyphDeviceType.Phone3 => Glyph.Device24111,
                    _ => throw new ArgumentException($"Unsupported device type: {deviceType}")
                };

                var success = _glyphManager.Register(deviceString);
                if (success)
                {
                    RegisteredDevice = deviceType;
                    Debug.WriteLine($"AndroidInterfaceService: Successfully registered for {deviceType}");
                }
                else
                {
                    Debug.WriteLine($"AndroidInterfaceService: Failed to register for {deviceType}");
                }
            });

            return RegisteredDevice.HasValue;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Registration failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Device registration failed", ex));
            return false;
        }
    }

    public async Task<bool> OpenSessionAsync()
    {
        try
        {
            if (_glyphManager == null)
            {
                Debug.WriteLine("AndroidInterfaceService: Cannot open session - service not initialized");
                return false;
            }

            if (!IsConnected)
            {
                Debug.WriteLine("AndroidInterfaceService: Cannot open session - service not connected");
                return false;
            }

            if (IsSessionOpen)
            {
                Debug.WriteLine("AndroidInterfaceService: Session already open");
                return true;
            }

            Debug.WriteLine("AndroidInterfaceService: Opening session...");

            await Task.Run(() => _glyphManager.OpenSession());
            IsSessionOpen = true;

            Debug.WriteLine("AndroidInterfaceService: Session opened successfully");
            return true;
        }
        catch (GlyphException ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Failed to open session - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Failed to open session", ex));
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Unexpected error opening session - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Unexpected error opening session", ex));
            return false;
        }
    }

    public async Task CloseSessionAsync()
    {
        try
        {
            if (_glyphManager == null || !IsSessionOpen)
            {
                Debug.WriteLine("AndroidInterfaceService: Session already closed or service not initialized");
                return;
            }

            Debug.WriteLine("AndroidInterfaceService: Closing session...");

            await Task.Run(() => _glyphManager.CloseSession());
            IsSessionOpen = false;

            Debug.WriteLine("AndroidInterfaceService: Session closed successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Failed to close session - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Failed to close session", ex));
        }
    }

    public async Task ShutdownAsync()
    {
        try
        {
            Debug.WriteLine("AndroidInterfaceService: Starting shutdown...");

            await CloseSessionAsync();

            if (_glyphManager != null)
            {
                await Task.Run(() => _glyphManager.UnInit());
                _glyphManager = null;
            }

            IsConnected = false;
            RegisteredDevice = null;
            ConnectionChanged?.Invoke(this, false);

            Debug.WriteLine("AndroidInterfaceService: Shutdown completed");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Shutdown failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Shutdown failed", ex));
        }
    }

    #endregion

    #region Glyph Control Operations

    public async Task<bool> ToggleChannelsAsync(params int[] channels)
    {
        if (!await EnsureSessionReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidInterfaceService: Toggling channels [{string.Join(", ", channels)}]");

            var frame = await Task.Run(() =>
            {
                var builder = _glyphManager!.GlyphFrameBuilder;
                foreach (var channel in channels)
                {
                    builder.BuildChannel(channel);
                }
                return builder.Build();
            });

            await Task.Run(() => _glyphManager!.Toggle(frame));

            Debug.WriteLine("AndroidInterfaceService: Toggle completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Toggle failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Toggle operation failed", ex));
            return false;
        }
    }

    public async Task<bool> AnimateChannelsAsync(int[] channels, int period = 1000, int cycles = 1, int interval = 0)
    {
        if (!await EnsureSessionReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidInterfaceService: Animating channels [{string.Join(", ", channels)}] - Period: {period}ms, Cycles: {cycles}, Interval: {interval}ms");

            var frame = await Task.Run(() =>
            {
                var builder = _glyphManager!.GlyphFrameBuilder;
                foreach (var channel in channels)
                {
                    builder.BuildChannel(channel);
                }
                return builder
                    .BuildPeriod(period)
                    .BuildCycles(cycles)
                    .BuildInterval(interval)
                    .Build();
            });

            await Task.Run(() => _glyphManager!.Animate(frame));

            Debug.WriteLine("AndroidInterfaceService: Animation started successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Animation failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Animation operation failed", ex));
            return false;
        }
    }

    public async Task<bool> DisplayProgressAsync(int[] channels, int progress, bool reverse = false)
    {
        if (!await EnsureSessionReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidInterfaceService: Displaying progress {progress}% (reverse: {reverse})");

            var frame = await Task.Run(() =>
            {
                var builder = _glyphManager!.GlyphFrameBuilder;
                foreach (var channel in channels)
                {
                    builder.BuildChannel(channel);
                }
                return builder.Build();
            });

            await Task.Run(() => _glyphManager!.DisplayProgress(frame, progress, reverse));

            Debug.WriteLine("AndroidInterfaceService: Progress display completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Progress display failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Progress display failed", ex));
            return false;
        }
    }

    public async Task<bool> DisplayProgressAndToggleAsync(int[] channels, int progress, bool reverse = false)
    {
        if (!await EnsureSessionReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidInterfaceService: Displaying progress {progress}% with toggle (reverse: {reverse})");

            var frame = await Task.Run(() =>
            {
                var builder = _glyphManager!.GlyphFrameBuilder;
                foreach (var channel in channels)
                {
                    builder.BuildChannel(channel);
                }
                return builder.Build();
            });

            await Task.Run(() => _glyphManager!.DisplayProgressAndToggle(frame, progress, reverse));

            Debug.WriteLine("AndroidInterfaceService: Progress and toggle completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Progress and toggle failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Progress and toggle failed", ex));
            return false;
        }
    }

    public async Task TurnOffAsync()
    {
        if (!await EnsureSessionReady()) return;

        try
        {
            Debug.WriteLine("AndroidInterfaceService: Turning off all lights...");

            await Task.Run(() => _glyphManager!.TurnOff());

            Debug.WriteLine("AndroidInterfaceService: All lights turned off successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidInterfaceService: Turn off failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Turn off operation failed", ex));
        }
    }

    #endregion

    #region Frame Builder Access

    public IGlyphFrameBuilder CreateFrameBuilder()
    {
        if (_glyphManager == null)
            throw new InvalidOperationException("Service not initialized");

        return new GlyphFrameBuilderWrapper(_glyphManager.GlyphFrameBuilder);
    }

    #endregion

    #region Helper Methods

    private async Task<bool> EnsureSessionReady()
    {
        if (_glyphManager == null)
        {
            Debug.WriteLine("AndroidInterfaceService: Service not initialized");
            return false;
        }

        if (!IsConnected)
        {
            Debug.WriteLine("AndroidInterfaceService: Service not connected");
            return false;
        }

        if (!IsSessionOpen)
        {
            Debug.WriteLine("AndroidInterfaceService: No active session - attempting to open...");
            return await OpenSessionAsync();
        }

        return true;
    }

    #endregion

    #region Internal Callback Handler

    private class GlyphManagerCallback(AndroidInterfaceService service) : Java.Lang.Object, GlyphManager.ICallback
    {
        private readonly AndroidInterfaceService _service = service;

        public void OnServiceConnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidInterfaceService: Glyph service connected");
            _service.IsConnected = true;
            _service.ConnectionChanged?.Invoke(_service, true);
            _service._connectionTcs?.SetResult(true);
        }

        public void OnServiceDisconnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidInterfaceService: Glyph service disconnected");
            _service.IsConnected = false;
            _service.IsSessionOpen = false;
            _service.ConnectionChanged?.Invoke(_service, false);
            _service._connectionTcs?.SetResult(false);
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (!_disposed)
        {
            _ = Task.Run(async () => await ShutdownAsync());
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}

#region Frame Builder Wrapper

/// <summary>
/// Wrapper for the native GlyphFrame.Builder
/// </summary>
internal class GlyphFrameBuilderWrapper(GlyphFrame.Builder nativeBuilder) : IGlyphFrameBuilder
{
    private readonly GlyphFrame.Builder _nativeBuilder = nativeBuilder;

    public IGlyphFrameBuilder AddChannel(int channel)
    {
        _nativeBuilder.BuildChannel(channel);
        return this;
    }

    public IGlyphFrameBuilder AddChannels(params int[] channels)
    {
        foreach (var channel in channels)
            _nativeBuilder.BuildChannel(channel);
        return this;
    }

    public IGlyphFrameBuilder AddChannelA()
    {
        _nativeBuilder.BuildChannelA();
        return this;
    }

    public IGlyphFrameBuilder AddChannelB()
    {
        _nativeBuilder.BuildChannelB();
        return this;
    }

    public IGlyphFrameBuilder AddChannelC()
    {
        _nativeBuilder.BuildChannelC();
        return this;
    }

    public IGlyphFrameBuilder AddChannelD()
    {
        _nativeBuilder.BuildChannelD();
        return this;
    }

    public IGlyphFrameBuilder AddChannelE()
    {
        _nativeBuilder.BuildChannelE();
        return this;
    }

    public IGlyphFrameBuilder SetPeriod(int milliseconds)
    {
        _nativeBuilder.BuildPeriod(milliseconds);
        return this;
    }

    public IGlyphFrameBuilder SetCycles(int cycles)
    {
        _nativeBuilder.BuildCycles(cycles);
        return this;
    }

    public IGlyphFrameBuilder SetInterval(int milliseconds)
    {
        _nativeBuilder.BuildInterval(milliseconds);
        return this;
    }

    public IGlyphFrame Build()
    {
        var nativeFrame = _nativeBuilder.Build();
        return new GlyphFrameWrapper(nativeFrame);
    }
}

/// <summary>
/// Wrapper for the native GlyphFrame
/// </summary>
internal class GlyphFrameWrapper(GlyphFrame nativeFrame) : IGlyphFrame
{
    private readonly GlyphFrame _nativeFrame = nativeFrame;

    public int[] Channels => _nativeFrame.GetChannel() ?? [];
    public int Period => _nativeFrame.Period;
    public int Cycles => _nativeFrame.Cycles;
    public int Interval => _nativeFrame.Interval;
}

#endregion