
/// <summary>
/// Represents a complete matrix frame with multiple layers
/// </summary>
public interface IGlyphMatrixFrame
{
    IGlyphMatrixObject? TopLayer { get; }
    IGlyphMatrixObject? MiddleLayer { get; }
    IGlyphMatrixObject? BottomLayer { get; }

    int[] Render();
}
