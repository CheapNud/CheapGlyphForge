// CheapGlyphForge.Core/Interfaces/IGlyphMatrixService.cs
namespace CheapGlyphForge.Core.Interfaces;

public interface IGlyphMatrixService
{
    Task<bool> InitializeAsync();
    Task<bool> RegisterDeviceAsync();
    Task<bool> SetMatrixFrameAsync(int[] ledData);
    Task TurnOffAsync();
    Task ShutdownAsync();

    event EventHandler<bool> ConnectionChanged;
}