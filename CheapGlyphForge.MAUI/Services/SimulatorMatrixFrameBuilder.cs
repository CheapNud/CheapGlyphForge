// CheapGlyphForge.MAUI/Services/SimulatorMatrixFrameBuilder.cs
using CheapGlyphForge.Core.Interfaces;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Services;

/// <summary>
/// Simulator implementation of IGlyphMatrixFrameBuilder
/// </summary>
internal sealed class SimulatorMatrixFrameBuilder : IGlyphMatrixFrameBuilder
{
    private IGlyphMatrixObject? _topLayer;
    private IGlyphMatrixObject? _middleLayer;
    private IGlyphMatrixObject? _bottomLayer;

    public IGlyphMatrixFrameBuilder AddTopLayer(IGlyphMatrixObject obj)
    {
        _topLayer = obj;
        Debug.WriteLine($"SimulatorMatrixFrameBuilder: Added top layer object");
        return this;
    }

    public IGlyphMatrixFrameBuilder AddMiddleLayer(IGlyphMatrixObject obj)
    {
        _middleLayer = obj;
        Debug.WriteLine($"SimulatorMatrixFrameBuilder: Added middle layer object");
        return this;
    }

    public IGlyphMatrixFrameBuilder AddBottomLayer(IGlyphMatrixObject obj)
    {
        _bottomLayer = obj;
        Debug.WriteLine($"SimulatorMatrixFrameBuilder: Added bottom layer object");
        return this;
    }

    public IGlyphMatrixFrame Build()
    {
        Debug.WriteLine($"SimulatorMatrixFrameBuilder: Building layered matrix frame");
        return new SimulatorMatrixFrame(_topLayer, _middleLayer, _bottomLayer);
    }
}
