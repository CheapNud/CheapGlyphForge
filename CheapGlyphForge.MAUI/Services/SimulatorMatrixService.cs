// CheapGlyphForge.MAUI/Services/SimulatorMatrixService.cs
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphMatrixService for cross-platform development
/// Provides same interface as AndroidMatrixService but simulates 25x25 LED matrix behavior
/// </summary>
public sealed class SimulatorMatrixService : IGlyphMatrixService
{
    private const int SimulationDelayMs = 50;
    private const string ServiceName = "SimulatorMatrixService";

    private bool _isConnected;
    private GlyphDeviceType? _registeredDevice;
    private readonly int[] _currentMatrix = new int[IGlyphMatrixService.TotalPixels];
    private readonly Random _random = new();

    #region Events
    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<GlyphErrorEventArgs>? ErrorOccurred;
    public event EventHandler<GlyphMatrixUpdateEventArgs>? FrameUpdated;
    #endregion

    #region Properties
    public bool IsConnected => _isConnected;
    public GlyphDeviceType? RegisteredDevice => _registeredDevice;
    public int[] CurrentMatrix => (int[])_currentMatrix.Clone();
    #endregion

    #region Connection Management
    public async Task<bool> InitializeAsync()
    {
        Debug.WriteLine($"{ServiceName}: Initializing matrix simulator...");

        // Simulate initialization delay
        await Task.Delay(SimulationDelayMs);

        _isConnected = true;
        ConnectionChanged?.Invoke(this, true);

        Debug.WriteLine($"{ServiceName}: Matrix simulator initialized successfully");
        return true;
    }

    public async Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType)
    {
        Debug.WriteLine($"{ServiceName}: Registering device {deviceType} for matrix control...");

        await Task.Delay(SimulationDelayMs);

        _registeredDevice = deviceType;
        Debug.WriteLine($"{ServiceName}: Device {deviceType} registered for matrix successfully");
        return true;
    }

    public async Task ShutdownAsync()
    {
        Debug.WriteLine($"{ServiceName}: Shutting down matrix simulator...");

        Array.Clear(_currentMatrix);
        _isConnected = false;
        _registeredDevice = null;
        ConnectionChanged?.Invoke(this, false);

        Debug.WriteLine($"{ServiceName}: Matrix simulator shutdown completed");
        await Task.CompletedTask;
    }
    #endregion

    #region Raw Matrix Control
    public GlyphPixel[] GetCurrentPixels()
    {
        var pixels = new GlyphPixel[IGlyphMatrixService.TotalPixels];

        for (int i = 0; i < IGlyphMatrixService.TotalPixels; i++)
        {
            var x = i % IGlyphMatrixService.MatrixWidth;
            var y = i / IGlyphMatrixService.MatrixWidth;
            pixels[i] = new GlyphPixel(x, y, _currentMatrix[i]);
        }

        return pixels;
    }

    public async Task<bool> SetMatrixFrameAsync(int[] ledData)
    {
        if (!ValidateMatrixData(ledData))
        {
            Debug.WriteLine($"{ServiceName}: Invalid matrix data provided");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs("Invalid matrix data - must be 625 elements"));
            return false;
        }

        Debug.WriteLine($"{ServiceName}: Setting matrix frame from raw data...");

        await Task.Delay(SimulationDelayMs);

        Array.Copy(ledData, _currentMatrix, IGlyphMatrixService.TotalPixels);

        var activePixels = ledData.Count(intensity => intensity > 0);
        FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs("Matrix frame updated", activePixels));

        Debug.WriteLine($"{ServiceName}: Matrix frame set successfully ({activePixels} active pixels)");
        return true;
    }

    public async Task<bool> SetPixelAsync(int x, int y, int intensity)
    {
        if (!IsValidCoordinate(x, y))
        {
            Debug.WriteLine($"{ServiceName}: Invalid coordinates ({x}, {y})");
            ErrorOccurred?.Invoke(this, new GlyphErrorEventArgs($"Invalid coordinates: ({x}, {y})"));
            return false;
        }

        Debug.WriteLine($"{ServiceName}: Setting pixel ({x}, {y}) to intensity {intensity}");

        await Task.Delay(SimulationDelayMs);

        var index = CoordinateToIndex(x, y);
        _currentMatrix[index] = Math.Clamp(intensity, 0, 255);

        FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs($"Pixel ({x}, {y}) updated", 1));

        Debug.WriteLine($"{ServiceName}: Pixel set successfully");
        return true;
    }

    public async Task<bool> SetPixelsAsync(IEnumerable<GlyphPixel> pixels)
    {
        Debug.WriteLine($"{ServiceName}: Setting multiple pixels...");

        await Task.Delay(SimulationDelayMs);

        var validPixels = 0;
        foreach (var pixel in pixels)
        {
            if (IsValidCoordinate(pixel.X, pixel.Y))
            {
                var index = CoordinateToIndex(pixel.X, pixel.Y);
                _currentMatrix[index] = Math.Clamp(pixel.Intensity, 0, 255);
                validPixels++;
            }
        }

        FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs("Multiple pixels updated", validPixels));

        Debug.WriteLine($"{ServiceName}: {validPixels} pixels set successfully");
        return validPixels > 0;
    }

    public async Task TurnOffAsync()
    {
        Debug.WriteLine($"{ServiceName}: Turning off matrix...");

        await Task.Delay(SimulationDelayMs);

        Array.Clear(_currentMatrix);
        FrameUpdated?.Invoke(this, new GlyphMatrixUpdateEventArgs("Matrix cleared", 0));

        Debug.WriteLine($"{ServiceName}: Matrix turned off successfully");
    }
    #endregion

    #region Frame-Based Operations
    public async Task<bool> SetMatrixFrameAsync(IGlyphMatrixFrame frame)
    {
        Debug.WriteLine($"{ServiceName}: Setting matrix frame from IGlyphMatrixFrame...");

        await Task.Delay(SimulationDelayMs);

        // Use the frame's render method to get raw data
        var rawData = frame.Render();
        return await SetMatrixFrameAsync(rawData);
    }

    public IGlyphMatrixFrameBuilder CreateFrameBuilder()
    {
        Debug.WriteLine($"{ServiceName}: Creating simulator matrix frame builder");
        return new SimulatorMatrixFrameBuilder();
    }
    #endregion

    #region High-Level Drawing Operations
    public async Task<bool> DrawTextAsync(string text, GlyphTextStyle style = GlyphTextStyle.Default, GlyphMarqueeType marquee = GlyphMarqueeType.None)
    {
        Debug.WriteLine($"{ServiceName}: Drawing text '{text}' with style {style} and marquee {marquee}");

        await Task.Delay(SimulationDelayMs * 2);

        // Simulate text rendering by creating a simple pattern
        var pixels = GenerateTextPattern(text);
        return await SetPixelsAsync(pixels);
    }

    public async Task<bool> DrawShapeAsync(GlyphShape shape, int x = 0, int y = 0, int size = 5)
    {
        Debug.WriteLine($"{ServiceName}: Drawing {shape} at ({x}, {y}) with size {size}");

        await Task.Delay(SimulationDelayMs);

        var pixels = shape switch
        {
            GlyphShape.Circle => GenerateCirclePixels(x, y, size),
            GlyphShape.Square => GenerateSquarePixels(x, y, size),
            GlyphShape.Line => GenerateLinePixels(x, y, size, true),
            GlyphShape.Cross => GenerateCrossPixels(x, y, size),
            _ => []
        };

        return await SetPixelsAsync(pixels);
    }

    public async Task<bool> PlayAnimationAsync(GlyphAnimation animation, int duration = 1000)
    {
        Debug.WriteLine($"{ServiceName}: Playing {animation} animation for {duration}ms");

        await Task.Delay(SimulationDelayMs);

        // Simulate animation by creating animated patterns
        _ = Task.Run(async () =>
        {
            var steps = Math.Max(10, duration / 100);
            for (int step = 0; step < steps; step++)
            {
                var pixels = GenerateAnimationFrame(animation, step, steps);
                await SetPixelsAsync(pixels);
                await Task.Delay(duration / steps);
            }
        });

        Debug.WriteLine($"{ServiceName}: Animation started successfully");
        return true;
    }
    #endregion

    #region Matrix Properties and Utilities
    public Task<int[]?> GetCurrentMatrixAsync()
    {
        Debug.WriteLine($"{ServiceName}: Getting current matrix state...");
        var matrixCopy = new int[IGlyphMatrixService.TotalPixels];
        Array.Copy(_currentMatrix, matrixCopy, IGlyphMatrixService.TotalPixels);
        return Task.FromResult<int[]?>(matrixCopy);
    }

    public bool ValidateMatrixData(int[] ledData)
    {
        return ledData != null && ledData.Length == IGlyphMatrixService.TotalPixels;
    }
    #endregion

    #region Helper Methods
    private static bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < IGlyphMatrixService.MatrixWidth && y >= 0 && y < IGlyphMatrixService.MatrixHeight;
    }

    private static int CoordinateToIndex(int x, int y) => IGlyphMatrixService.CoordinateToIndex(x, y);

    private static (int x, int y) IndexToCoordinate(int index) => IGlyphMatrixService.IndexToCoordinate(index);

    // Pattern Generation Methods
    private static IEnumerable<GlyphPixel> GenerateCirclePixels(int centerX, int centerY, int radius)
    {
        var pixels = new List<GlyphPixel>();

        for (int x = Math.Max(0, centerX - radius); x <= Math.Min(IGlyphMatrixService.MatrixWidth - 1, centerX + radius); x++)
        {
            for (int y = Math.Max(0, centerY - radius); y <= Math.Min(IGlyphMatrixService.MatrixHeight - 1, centerY + radius); y++)
            {
                var distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                if (distance <= radius)
                {
                    pixels.Add(new GlyphPixel(x, y, 255));
                }
            }
        }

        return pixels;
    }

    private static IEnumerable<GlyphPixel> GenerateSquarePixels(int startX, int startY, int size)
    {
        var pixels = new List<GlyphPixel>();

        for (int x = startX; x < startX + size && x < IGlyphMatrixService.MatrixWidth; x++)
        {
            for (int y = startY; y < startY + size && y < IGlyphMatrixService.MatrixHeight; y++)
            {
                if (x >= 0 && y >= 0)
                {
                    pixels.Add(new GlyphPixel(x, y, 255));
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
            for (int x = startX; x < startX + length && x < IGlyphMatrixService.MatrixWidth; x++)
            {
                if (x >= 0 && startY >= 0 && startY < IGlyphMatrixService.MatrixHeight)
                {
                    pixels.Add(new GlyphPixel(x, startY, 255));
                }
            }
        }
        else
        {
            for (int y = startY; y < startY + length && y < IGlyphMatrixService.MatrixHeight; y++)
            {
                if (startX >= 0 && startX < IGlyphMatrixService.MatrixWidth && y >= 0)
                {
                    pixels.Add(new GlyphPixel(startX, y, 255));
                }
            }
        }

        return pixels;
    }

    private static IEnumerable<GlyphPixel> GenerateCrossPixels(int centerX, int centerY, int size)
    {
        var pixels = new List<GlyphPixel>();
        var halfSize = size / 2;

        // Horizontal line
        pixels.AddRange(GenerateLinePixels(centerX - halfSize, centerY, size, true));

        // Vertical line
        pixels.AddRange(GenerateLinePixels(centerX, centerY - halfSize, size, false));

        return pixels;
    }

    private IEnumerable<GlyphPixel> GenerateTextPattern(string text)
    {
        var pixels = new List<GlyphPixel>();

        // Simple text simulation - create a pattern based on text length
        var centerX = IGlyphMatrixService.MatrixWidth / 2;
        var centerY = IGlyphMatrixService.MatrixHeight / 2;

        for (int i = 0; i < text.Length && i < 12; i++)
        {
            var x = centerX - (text.Length / 2) + i;
            var y = centerY + (char.IsUpper(text[i]) ? -1 : 0);

            if (IsValidCoordinate(x, y))
            {
                pixels.Add(new GlyphPixel(x, y, 200));
            }
        }

        return pixels;
    }

    private IEnumerable<GlyphPixel> GenerateAnimationFrame(GlyphAnimation animation, int step, int totalSteps)
    {
        var progress = (double)step / totalSteps;
        var centerX = IGlyphMatrixService.MatrixWidth / 2;
        var centerY = IGlyphMatrixService.MatrixHeight / 2;

        return animation switch
        {
            GlyphAnimation.Pulse => GenerateCirclePixels(centerX, centerY, (int)(10 * Math.Sin(progress * Math.PI * 2))),
            GlyphAnimation.Wave => GenerateWavePixels(progress),
            GlyphAnimation.Spiral => GenerateSpiralPixels(progress),
            _ => []
        };
    }

    private IEnumerable<GlyphPixel> GenerateWavePixels(double progress)
    {
        var pixels = new List<GlyphPixel>();

        for (int x = 0; x < IGlyphMatrixService.MatrixWidth; x++)
        {
            var waveHeight = (int)(Math.Sin((x + progress * 50) * 0.5) * 5 + IGlyphMatrixService.MatrixHeight / 2);
            if (waveHeight >= 0 && waveHeight < IGlyphMatrixService.MatrixHeight)
            {
                pixels.Add(new GlyphPixel(x, waveHeight, 200));
            }
        }

        return pixels;
    }

    private IEnumerable<GlyphPixel> GenerateSpiralPixels(double progress)
    {
        var pixels = new List<GlyphPixel>();
        var centerX = IGlyphMatrixService.MatrixWidth / 2;
        var centerY = IGlyphMatrixService.MatrixHeight / 2;
        var maxRadius = Math.Min(centerX, centerY);

        for (double angle = 0; angle < Math.PI * 4; angle += 0.3)
        {
            var radius = (angle / (Math.PI * 4)) * maxRadius * progress;
            var x = (int)(centerX + radius * Math.Cos(angle));
            var y = (int)(centerY + radius * Math.Sin(angle));

            if (IsValidCoordinate(x, y))
            {
                pixels.Add(new GlyphPixel(x, y, (int)(255 * (1 - angle / (Math.PI * 4)))));
            }
        }

        return pixels;
    }
    #endregion
}
