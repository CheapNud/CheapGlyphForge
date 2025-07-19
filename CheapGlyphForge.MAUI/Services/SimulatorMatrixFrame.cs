// CheapGlyphForge.MAUI/Services/SimulatorMatrixFrame.cs
using CheapGlyphForge.Core.Interfaces;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphMatrixFrame
/// </summary>
internal sealed record SimulatorMatrixFrame(
    IGlyphMatrixObject? TopLayer,
    IGlyphMatrixObject? MiddleLayer,
    IGlyphMatrixObject? BottomLayer) : IGlyphMatrixFrame
{
    public int[] Render()
    {
        var matrix = new int[625]; // 25x25

        // Render layers from bottom to top
        RenderLayer(matrix, BottomLayer);
        RenderLayer(matrix, MiddleLayer);
        RenderLayer(matrix, TopLayer);

        return matrix;
    }

    public int[] GetRawData() => Render();

    private static void RenderLayer(int[] matrix, IGlyphMatrixObject? layer)
    {
        if (layer == null) return;

        // Simple text rendering simulation
        if (!string.IsNullOrEmpty(layer.Text))
        {
            var text = layer.Text;
            var centerX = Math.Clamp(layer.PositionX, 0, 24);
            var centerY = Math.Clamp(layer.PositionY, 0, 24);

            // Simple character rendering
            for (int i = 0; i < Math.Min(text.Length, 12); i++)
            {
                var x = centerX - (text.Length / 2) + i;
                var y = centerY;

                if (x >= 0 && x < 25 && y >= 0 && y < 25)
                {
                    var index = y * 25 + x;
                    var intensity = (int)(layer.Brightness * (1 - layer.Transparency / 255.0));
                    matrix[index] = Math.Max(matrix[index], intensity);
                }
            }
        }
    }
}
