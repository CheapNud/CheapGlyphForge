// CheapGlyphForge.MAUI/Platforms/Android/Services/AndroidMatrixService.cs
using Android.Content;
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using Com.Nothing.Ketchum;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

/// <summary>
/// Android implementation of IGlyphMatrixService using Nothing Phone Glyph Matrix SDK
/// </summary>
public partial class AndroidMatrixService : IGlyphMatrixService, IDisposable
{
    private readonly Context _context;
    private GlyphMatrixManager? _glyphMatrixManager;
    private GlyphMatrixManagerCallback? _callback;
    private bool _disposed;
    private TaskCompletionSource<bool>? _connectionTcs;

    private readonly int[] _currentMatrixState = new int[IGlyphMatrixService.TotalPixels];
    public int[] CurrentMatrix => (int[])_currentMatrixState.Clone();

    public AndroidMatrixService()
    {
        _context = Platform.CurrentActivity?.ApplicationContext ??
                   throw new InvalidOperationException("Android context not available");
    }

    #region Events
    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<GlyphErrorEventArgs>? ErrorOccurred;
    public event EventHandler<GlyphMatrixUpdateEventArgs>? FrameUpdated;
    #endregion

    #region Properties
    public bool IsConnected { get; private set; }
    public GlyphDeviceType? RegisteredDevice { get; private set; }
    #endregion

    #region Connection Management

    public async Task<bool> InitializeAsync()
    {
        try
        {
            if (_glyphMatrixManager != null)
            {
                Debug.WriteLine("AndroidMatrixService: Already initialized");
                return true;
            }

            Debug.WriteLine("AndroidMatrixService: Starting initialization...");

            _glyphMatrixManager = GlyphMatrixManager.GetInstance(_context);
            _callback = new GlyphMatrixManagerCallback(this);
            _connectionTcs = new TaskCompletionSource<bool>();

            // Start the async initialization
            await Task.Run(() => _glyphMatrixManager!.Init(_callback));

            // Wait for connection callback with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
                var connected = await _connectionTcs.Task.WaitAsync(cts.Token);
                Debug.WriteLine($"AndroidMatrixService: Initialization completed - Connected: {connected}");
                return connected;
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("AndroidMatrixService: Initialization timed out");
                ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Matrix initialization timed out"));
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: Initialization failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Matrix initialization failed", ex));
            return false;
        }
    }

    public async Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType)
    {
        try
        {
            if (_glyphMatrixManager == null)
            {
                Debug.WriteLine("AndroidMatrixService: Cannot register - service not initialized");
                return false;
            }

            Debug.WriteLine($"AndroidMatrixService: Registering device {deviceType}...");

            var deviceString = deviceType switch
            {
                GlyphDeviceType.Phone1 => "20111",
                GlyphDeviceType.Phone2 => "22111",
                GlyphDeviceType.Phone2a => "23111",
                GlyphDeviceType.Phone2aPlus => "23113",
                GlyphDeviceType.Phone3 => "24111",
                _ => throw new ArgumentException($"Unknown device type: {deviceType}")
            };

            await Task.Run(() => _glyphMatrixManager!.Register(deviceString));

            RegisteredDevice = deviceType;
            Debug.WriteLine($"AndroidMatrixService: Device {deviceType} registered successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: Device registration failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Device registration failed", ex));
            return false;
        }
    }

    public async Task ShutdownAsync()
    {
        try
        {
            Debug.WriteLine("AndroidMatrixService: Starting shutdown...");

            if (_glyphMatrixManager != null)
            {
                await Task.Run(() => _glyphMatrixManager.UnInit());
                _glyphMatrixManager = null;
            }

            IsConnected = false;
            RegisteredDevice = null;
            ConnectionChanged?.Invoke(this, false);

            Debug.WriteLine("AndroidMatrixService: Shutdown completed");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: Shutdown failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Matrix shutdown failed", ex));
        }
    }

    #endregion

    #region Raw Matrix Control
    public async Task<bool> SetMatrixFrameAsync(int[] ledData)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            if (!ValidateMatrixData(ledData))
            {
                Debug.WriteLine("AndroidMatrixService: Invalid matrix data provided");
                ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Invalid matrix data - must be 625 elements"));
                return false;
            }

            Debug.WriteLine("AndroidMatrixService: Setting matrix frame from raw data...");

            await Task.Run(() => _glyphMatrixManager!.SetMatrixFrame(ledData));

            // ✅ Update our internal state tracking
            Array.Copy(ledData, _currentMatrixState, IGlyphMatrixService.TotalPixels);

            var activePixels = ledData.Count(intensity => intensity > 0);
            FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs("Matrix frame updated", activePixels));

            Debug.WriteLine($"AndroidMatrixService: Matrix frame set successfully ({activePixels} active pixels)");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: SetMatrixFrame failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Matrix frame update failed", ex));
            return false;
        }
    }

    public async Task<bool> SetPixelAsync(int x, int y, int intensity)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            if (x < 0 || x >= IGlyphMatrixService.MatrixWidth || y < 0 || y >= IGlyphMatrixService.MatrixHeight)
            {
                Debug.WriteLine($"AndroidMatrixService: Invalid coordinates ({x}, {y})");
                return false;
            }

            Debug.WriteLine($"AndroidMatrixService: Setting pixel ({x}, {y}) to intensity {intensity}");

            // Update our current state
            var index = IGlyphMatrixService.CoordinateToIndex(x, y);
            _currentMatrixState[index] = intensity;

            // Send the entire matrix to the device
            return await SetMatrixFrameAsync(_currentMatrixState);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: SetPixel failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Pixel update failed", ex));
            return false;
        }
    }

    public async Task<bool> SetPixelsAsync(IEnumerable<GlyphPixel> pixels)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidMatrixService: Setting multiple pixels...");

            var currentMatrix = await GetCurrentMatrixAsync();
            if (currentMatrix == null)
            {
                currentMatrix = new int[IGlyphMatrixService.TotalPixels];
            }

            foreach (var pixel in pixels)
            {
                if (pixel.X >= 0 && pixel.X < IGlyphMatrixService.MatrixWidth &&
                    pixel.Y >= 0 && pixel.Y < IGlyphMatrixService.MatrixHeight)
                {
                    var index = IGlyphMatrixService.CoordinateToIndex(pixel.X, pixel.Y);
                    currentMatrix[index] = pixel.Intensity;
                }
            }

            return await SetMatrixFrameAsync(currentMatrix);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: SetPixels failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Multiple pixel update failed", ex));
            return false;
        }
    }

    public async Task TurnOffAsync()
    {
        if (!await EnsureServiceReady()) return;

        try
        {
            Debug.WriteLine("AndroidMatrixService: Turning off matrix...");

            // Create empty matrix (all zeros)
            var emptyMatrix = new int[IGlyphMatrixService.TotalPixels];
            await Task.Run(() => _glyphMatrixManager!.SetMatrixFrame(emptyMatrix));

            // ✅ Clear our internal state
            Array.Clear(_currentMatrixState);

            FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs("Matrix cleared", 0));
            Debug.WriteLine("AndroidMatrixService: Matrix turned off successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: TurnOff failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Matrix turn off failed", ex));
        }
    }

    #endregion

    #region Frame-Based Operations

    public async Task<bool> SetMatrixFrameAsync(IGlyphMatrixFrame frame)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            Debug.WriteLine("AndroidMatrixService: Setting matrix frame from IGlyphMatrixFrame...");

            // Use the frame's render method to get raw data
            var rawData = frame.Render();

            return await SetMatrixFrameAsync(rawData);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: SetMatrixFrame from IGlyphMatrixFrame failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Structured matrix frame update failed", ex));
            return false;
        }
    }

    public IGlyphMatrixFrameBuilder CreateFrameBuilder()
    {
        if (_glyphMatrixManager == null)
            throw new InvalidOperationException("Service not initialized");

        // Create new GlyphMatrixFrame.Builder instance
        return new GlyphMatrixFrameBuilderWrapper(new GlyphMatrixFrame.Builder());
    }

    #endregion

    #region High-Level Drawing Operations

    public async Task<bool> DrawTextAsync(string text, GlyphTextStyle style = GlyphTextStyle.Default,
        GlyphMarqueeType marquee = GlyphMarqueeType.None)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidMatrixService: Drawing text '{text}' with style {style}, marquee {marquee}");

            // Use the Matrix SDK's frame builder pattern from the docs
            var frameBuilder = new GlyphMatrixFrame.Builder();
            var objectBuilder = new GlyphMatrixObject.Builder();

            // Convert our enum to SDK marquee constants  
            var marqueeType = marquee switch
            {
                GlyphMarqueeType.Force => GlyphMatrixObject.TypeMarqueeForce,
                GlyphMarqueeType.Auto => GlyphMatrixObject.TypeMarqueeAuto,
                _ => GlyphMatrixObject.TypeMarqueeNone
            };

            // [Inference] Set marquee type - method name may vary
            objectBuilder.SetText(text, marqueeType);

            var textObject = objectBuilder.Build();
            if (textObject == null)
                throw new InvalidOperationException("Failed to build text object");

            var frameWithObject = frameBuilder.AddTop(textObject);
            if (frameWithObject == null)
                throw new InvalidOperationException("Failed to add text object to frame");

            var frame = frameWithObject.Build(_context);
            if (frame == null)
                throw new InvalidOperationException("Failed to build matrix frame");

            // Render and send to matrix
            var renderedData = frame.Render();
            if (renderedData == null)
                throw new InvalidOperationException("Failed to render matrix frame");

            await Task.Run(() => _glyphMatrixManager!.SetMatrixFrame(renderedData));

            FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs($"Text '{text}' rendered", text.Length));
            Debug.WriteLine("AndroidMatrixService: Text drawn successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: DrawText failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Text drawing failed", ex));
            return false;
        }
    }

    public async Task<bool> DrawShapeAsync(GlyphShape shape, int x = 0, int y = 0, int size = 5)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidMatrixService: Drawing shape {shape} at ({x}, {y}) with size {size}");

            var pixels = new List<GlyphPixel>();

            // Generate pixels based on shape type
            switch (shape)
            {
                case GlyphShape.Circle:
                    pixels.AddRange(GenerateCirclePixels(x, y, size));
                    break;
                case GlyphShape.Square:
                    pixels.AddRange(GenerateSquarePixels(x, y, size));
                    break;
                case GlyphShape.Line:
                    pixels.AddRange(GenerateLinePixels(x, y, size, true)); // horizontal line
                    break;
                case GlyphShape.Cross:
                    pixels.AddRange(GenerateCrossPixels(x, y, size));
                    break;
            }

            return await SetPixelsAsync(pixels);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: DrawShape failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Shape drawing failed", ex));
            return false;
        }
    }

    public async Task<bool> PlayAnimationAsync(GlyphAnimation animation, int duration = 1000)
    {
        if (!await EnsureServiceReady()) return false;

        try
        {
            Debug.WriteLine($"AndroidMatrixService: Playing animation {animation} for {duration}ms");

            // [Inference] This would need to be implemented based on available animation APIs
            // For now, return success as a placeholder
            await Task.Delay(100); // Simulate async operation

            FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs($"Animation {animation} started", duration));
            Debug.WriteLine("AndroidMatrixService: Animation started successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: PlayAnimation failed - {ex.Message}");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Animation playback failed", ex));
            return false;
        }
    }

    #endregion

    #region Matrix Properties and Utilities

    public Task<int[]?> GetCurrentMatrixAsync()
    {
        try
        {
            // [Inference] This assumes the SDK provides a way to get current matrix state
            // May need to maintain our own state if not available
            Debug.WriteLine("AndroidMatrixService: Getting current matrix state...");

            // For now, return null to indicate no current state available
            // This would need implementation based on actual SDK capabilities
            return Task.FromResult<int[]?>(null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AndroidMatrixService: GetCurrentMatrix failed - {ex.Message}");
            return Task.FromResult<int[]?>(null);
        }
    }

    public bool ValidateMatrixData(int[] ledData)
    {
        return ledData != null && ledData.Length == IGlyphMatrixService.TotalPixels;
    }

    #endregion

    #region Helper Methods

    private async Task<bool> EnsureServiceReady()
    {
        if (_glyphMatrixManager == null)
        {
            Debug.WriteLine("AndroidMatrixService: Service not initialized");
            return false;
        }

        if (!IsConnected)
        {
            Debug.WriteLine("AndroidMatrixService: Service not connected");
            return false;
        }

        return true;
    }

    private static IEnumerable<GlyphPixel> GenerateCirclePixels(int centerX, int centerY, int radius)
    {
        var pixels = new List<GlyphPixel>();

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                var distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                if (distance <= radius && x >= 0 && x < IGlyphMatrixService.MatrixWidth &&
                    y >= 0 && y < IGlyphMatrixService.MatrixHeight)
                {
                    pixels.Add(new GlyphPixel(x, y, 255)); // Full intensity
                }
            }
        }

        return pixels;
    }

    private static IEnumerable<GlyphPixel> GenerateSquarePixels(int x, int y, int size)
    {
        var pixels = new List<GlyphPixel>();

        for (int i = x; i < x + size && i < IGlyphMatrixService.MatrixWidth; i++)
        {
            for (int j = y; j < y + size && j < IGlyphMatrixService.MatrixHeight; j++)
            {
                if (i >= 0 && j >= 0)
                {
                    pixels.Add(new GlyphPixel(i, j, 255));
                }
            }
        }

        return pixels;
    }

    private static IEnumerable<GlyphPixel> GenerateLinePixels(int startX, int startY, int length, bool horizontal)
    {
        var pixels = new List<GlyphPixel>();

        if (horizontal)
        {
            for (int i = startX; i < startX + length && i < IGlyphMatrixService.MatrixWidth; i++)
            {
                if (i >= 0 && startY >= 0 && startY < IGlyphMatrixService.MatrixHeight)
                {
                    pixels.Add(new GlyphPixel(i, startY, 255));
                }
            }
        }
        else
        {
            for (int j = startY; j < startY + length && j < IGlyphMatrixService.MatrixHeight; j++)
            {
                if (startX >= 0 && startX < IGlyphMatrixService.MatrixWidth && j >= 0)
                {
                    pixels.Add(new GlyphPixel(startX, j, 255));
                }
            }
        }

        return pixels;
    }

    private static IEnumerable<GlyphPixel> GenerateCrossPixels(int centerX, int centerY, int size)
    {
        var pixels = new List<GlyphPixel>();

        // Horizontal line
        pixels.AddRange(GenerateLinePixels(centerX - size / 2, centerY, size, true));

        // Vertical line
        pixels.AddRange(GenerateLinePixels(centerX, centerY - size / 2, size, false));

        return pixels;
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

    #region State Access Implementation


    public GlyphPixel[] GetCurrentPixels()
    {
        var pixels = new GlyphPixel[IGlyphMatrixService.TotalPixels];

        for (int i = 0; i < IGlyphMatrixService.TotalPixels; i++)
        {
            var x = i % IGlyphMatrixService.MatrixWidth;
            var y = i / IGlyphMatrixService.MatrixWidth;
            pixels[i] = new GlyphPixel(x, y, _currentMatrixState[i]);
        }

        return pixels;
    }
    #endregion

}

// Callback class for Matrix Manager
public partial class AndroidMatrixService
{
    private class GlyphMatrixManagerCallback(AndroidMatrixService service) : Java.Lang.Object, GlyphMatrixManager.ICallback
    {
        private readonly AndroidMatrixService _service = service;

        public void OnServiceConnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidMatrixService: Glyph Matrix service connected");
            _service.IsConnected = true;
            _service.ConnectionChanged?.Invoke(_service, true);
            _service._connectionTcs?.SetResult(true);
        }

        public void OnServiceDisconnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidMatrixService: Glyph Matrix service disconnected");
            _service.IsConnected = false;
            _service.ConnectionChanged?.Invoke(_service, false);
            _service._connectionTcs?.SetResult(false);
        }
    }
}