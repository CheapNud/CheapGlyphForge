// CheapGlyphForge.Core/Interfaces/IGlyphMatrixService.cs
using CheapGlyphForge.Core.Models;

namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Clean C# wrapper for Nothing Phone Glyph Matrix SDK
/// Provides async operations for controlling the 25x25 LED matrix
/// </summary>
public interface IGlyphMatrixService
{
    #region Connection Management
    /// <summary>
    /// Initialize the Glyph Matrix service connection
    /// </summary>
    Task<bool> InitializeAsync();

    /// <summary>
    /// Register the app for Matrix control on target device
    /// </summary>
    Task<bool> RegisterDeviceAsync(GlyphDeviceType deviceType);

    /// <summary>
    /// Complete shutdown and cleanup
    /// </summary>
    Task ShutdownAsync();
    #endregion

    #region Raw Matrix Control
    /// <summary>
    /// Set the entire 25x25 matrix using raw LED data
    /// </summary>
    /// <param name="ledData">625-element array (25x25) of LED intensities</param>
    Task<bool> SetMatrixFrameAsync(int[] ledData);

    /// <summary>
    /// Set a specific pixel in the matrix
    /// </summary>
    /// <param name="x">X coordinate (0-24)</param>
    /// <param name="y">Y coordinate (0-24)</param>
    /// <param name="intensity">LED intensity value</param>
    Task<bool> SetPixelAsync(int x, int y, int intensity);

    /// <summary>
    /// Set multiple pixels at once
    /// </summary>
    Task<bool> SetPixelsAsync(IEnumerable<GlyphPixel> pixels);

    /// <summary>
    /// Clear the entire matrix (turn off all LEDs)
    /// </summary>
    Task TurnOffAsync();
    #endregion

    #region Frame-Based Operations
    /// <summary>
    /// Render and display a structured matrix frame
    /// </summary>
    Task<bool> SetMatrixFrameAsync(IGlyphMatrixFrame frame);

    /// <summary>
    /// Create a new matrix frame builder
    /// </summary>
    IGlyphMatrixFrameBuilder CreateFrameBuilder();
    #endregion

    #region High-Level Drawing Operations
    /// <summary>
    /// Draw text on the matrix with optional marquee
    /// </summary>
    Task<bool> DrawTextAsync(string text, GlyphTextStyle style = GlyphTextStyle.Default,
        GlyphMarqueeType marquee = GlyphMarqueeType.None);

    /// <summary>
    /// Draw a simple shape or pattern
    /// </summary>
    Task<bool> DrawShapeAsync(GlyphShape shape, int x = 0, int y = 0, int size = 5);

    /// <summary>
    /// Display a predefined animation pattern
    /// </summary>
    Task<bool> PlayAnimationAsync(GlyphAnimation animation, int duration = 1000);
    #endregion

    #region Matrix Properties and Utilities
    /// <summary>
    /// Get current matrix state as raw data
    /// </summary>
    Task<int[]?> GetCurrentMatrixAsync();

    /// <summary>
    /// Validate matrix data before sending
    /// </summary>
    bool ValidateMatrixData(int[] ledData);

    /// <summary>
    /// Convert 2D coordinates to flat array index
    /// </summary>
    static int CoordinateToIndex(int x, int y) => (y * MatrixWidth) + x;

    /// <summary>
    /// Convert flat array index to 2D coordinates
    /// </summary>
    static (int x, int y) IndexToCoordinate(int index) => (index % MatrixWidth, index / MatrixWidth);
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

    /// <summary>
    /// Fires when matrix frame updates complete
    /// </summary>
    event EventHandler<GlyphMatrixUpdateEventArgs> FrameUpdated;
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
    /// Matrix dimensions - typically 25x25
    /// </summary>
    const int MatrixWidth = 25;
    const int MatrixHeight = 25;
    const int TotalPixels = MatrixWidth * MatrixHeight;
    #endregion
}