// CheapGlyphForge.MAUI/Platforms/Android/Services/GlyphMatrixFrameBuilderWrapper.cs
using Android.Content;
using CheapGlyphForge.Core.Interfaces;
using Com.Nothing.Ketchum;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

// Wrapper classes for builders
public class GlyphMatrixFrameBuilderWrapper(GlyphMatrixFrame.Builder builder, Context? context) : IGlyphMatrixFrameBuilder
{
    private readonly Context _context = context ?? Platform.CurrentActivity?.ApplicationContext
                   ?? throw new InvalidOperationException("Android context not available");

    public GlyphMatrixFrameBuilderWrapper(GlyphMatrixFrame.Builder builder)
        : this(builder, Platform.CurrentActivity?.ApplicationContext)
    {
    }

    public IGlyphMatrixFrameBuilder AddTopLayer(IGlyphMatrixObject obj)
    {
        // [Inference] Convert IGlyphMatrixObject to native GlyphMatrixObject
        // For now, throw as implementation is needed
        throw new NotImplementedException("IGlyphMatrixObject to native conversion needed");
    }

    public IGlyphMatrixFrameBuilder AddMiddleLayer(IGlyphMatrixObject obj)
    {
        // [Inference] Convert IGlyphMatrixObject to native GlyphMatrixObject  
        // For now, throw as implementation is needed
        throw new NotImplementedException("IGlyphMatrixObject to native conversion needed");
    }

    public IGlyphMatrixFrameBuilder AddBottomLayer(IGlyphMatrixObject obj)
    {
        // [Inference] Convert IGlyphMatrixObject to native GlyphMatrixObject
        // For now, throw as implementation is needed  
        throw new NotImplementedException("IGlyphMatrixObject to native conversion needed");
    }

    public IGlyphMatrixFrame Build()
    {
        var nativeFrame = builder.Build(_context);
        return new GlyphMatrixFrameWrapper(nativeFrame);
    }
}