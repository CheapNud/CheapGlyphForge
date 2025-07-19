// CheapGlyphForge.MAUI/Platforms/Android/Services/GlyphMatrixFrameWrapper.cs
using CheapGlyphForge.Core.Interfaces;
using Com.Nothing.Ketchum;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

public class GlyphMatrixFrameWrapper(GlyphMatrixFrame nativeFrame) : IGlyphMatrixFrame
{
    public IGlyphMatrixObject? TopLayer => null; // [Inference] Would need native frame inspection
    public IGlyphMatrixObject? MiddleLayer => null; // [Inference] Would need native frame inspection  
    public IGlyphMatrixObject? BottomLayer => null; // [Inference] Would need native frame inspection

    public int[] Render()
    {
        return nativeFrame.Render() ?? new int[IGlyphMatrixService.TotalPixels];
    }

    public int[] GetRawData()
    {
        return Render();
    }
}