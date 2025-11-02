// CheapGlyphForge.MAUI/Platforms/Android/Services/GlyphMatrixFrameBuilderWrapper.cs
using Android.Content;
using CheapGlyphForge.Core.Interfaces;
using CheapGlyphForge.Core.Models;
using Com.Nothing.Ketchum;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

/// <summary>
/// Wrapper for native GlyphMatrixFrame.Builder that implements our clean IGlyphMatrixFrameBuilder interface
/// Converts our IGlyphMatrixObject to native GlyphMatrixObject using the SDK's builder pattern
/// </summary>
public class GlyphMatrixFrameBuilderWrapper : IGlyphMatrixFrameBuilder
{
    private readonly GlyphMatrixFrame.Builder _builder;
    private readonly Context _context;

    public GlyphMatrixFrameBuilderWrapper(GlyphMatrixFrame.Builder builder, Context? context = null)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        _context = context ?? Platform.CurrentActivity?.ApplicationContext
                   ?? throw new InvalidOperationException("Android context not available");
    }

    public IGlyphMatrixFrameBuilder AddTopLayer(IGlyphMatrixObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        try
        {
            var nativeObject = ConvertToNativeObject(obj);
            _builder.AddTop(nativeObject);
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Added top layer object at ({obj.PositionX}, {obj.PositionY})");
            return this;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Failed to add top layer - {ex.Message}");
            throw;
        }
    }

    public IGlyphMatrixFrameBuilder AddMiddleLayer(IGlyphMatrixObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        try
        {
            var nativeObject = ConvertToNativeObject(obj);
            _builder.AddMid(nativeObject);
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Added middle layer object at ({obj.PositionX}, {obj.PositionY})");
            return this;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Failed to add middle layer - {ex.Message}");
            throw;
        }
    }

    public IGlyphMatrixFrameBuilder AddBottomLayer(IGlyphMatrixObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        try
        {
            var nativeObject = ConvertToNativeObject(obj);
            _builder.AddLow(nativeObject);
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Added bottom layer object at ({obj.PositionX}, {obj.PositionY})");
            return this;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Failed to add bottom layer - {ex.Message}");
            throw;
        }
    }

    public IGlyphMatrixFrame Build()
    {
        try
        {
            var nativeFrame = _builder.Build(_context);
            if (nativeFrame == null)
                throw new InvalidOperationException("Failed to build native GlyphMatrixFrame");

            Debug.WriteLine("GlyphMatrixFrameBuilderWrapper: Successfully built matrix frame");
            return new GlyphMatrixFrameWrapper(nativeFrame);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GlyphMatrixFrameBuilderWrapper: Failed to build frame - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Convert our clean IGlyphMatrixObject to the native GlyphMatrixObject using the SDK's builder
    /// </summary>
    private static GlyphMatrixObject ConvertToNativeObject(IGlyphMatrixObject obj)
    {
        var builder = new GlyphMatrixObject.Builder();

        // Set position - always available
        builder.SetPosition(obj.PositionX, obj.PositionY);

        // Set orientation if not default
        if (obj.Orientation != 0)
        {
            builder.SetOrientation(obj.Orientation);
        }

        // Set scale if not default (100)
        if (obj.Scale != 100)
        {
            builder.SetScale(obj.Scale);
        }

        // Set brightness if not default (255)
        if (obj.Brightness != 255)
        {
            builder.SetBrightness(obj.Brightness);
        }

        // Set text if available
        if (!string.IsNullOrEmpty(obj.Text))
        {
            if (obj.MarqueeType != GlyphMarqueeType.None)
            {
                // Convert our enum to native marquee type
                var nativeMarqueeType = ConvertMarqueeType(obj.MarqueeType);
                builder.SetText(obj.Text, nativeMarqueeType);
            }
            else
            {
                builder.SetText(obj.Text);
            }
        }

        // Build and return the native object
        var nativeObject = builder.Build();
        if (nativeObject == null)
            throw new InvalidOperationException("Failed to build native GlyphMatrixObject");

        return nativeObject;
    }

    /// <summary>
    /// Convert our GlyphMarqueeType enum to the native SDK constants
    /// </summary>
    private static int ConvertMarqueeType(GlyphMarqueeType marqueeType)
    {
        return marqueeType switch
        {
            GlyphMarqueeType.None => GlyphMatrixObject.TypeMarqueeNone,
            GlyphMarqueeType.Force => GlyphMatrixObject.TypeMarqueeForce,
            GlyphMarqueeType.Auto => GlyphMatrixObject.TypeMarqueeAuto,
            _ => GlyphMatrixObject.TypeMarqueeNone
        };
    }
}