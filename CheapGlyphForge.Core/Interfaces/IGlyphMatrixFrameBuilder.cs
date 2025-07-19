namespace CheapGlyphForge.Core.Interfaces;

/// <summary>
/// Builder for creating layered matrix frames with objects
/// </summary>
public interface IGlyphMatrixFrameBuilder
{
    IGlyphMatrixFrameBuilder AddTopLayer(IGlyphMatrixObject obj);
    IGlyphMatrixFrameBuilder AddMiddleLayer(IGlyphMatrixObject obj);
    IGlyphMatrixFrameBuilder AddBottomLayer(IGlyphMatrixObject obj);

    IGlyphMatrixFrame Build();
}
