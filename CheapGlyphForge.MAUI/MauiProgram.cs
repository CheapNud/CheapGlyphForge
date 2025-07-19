using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using CheapGlyphForge.Core.Interfaces;

#if ANDROID
using CheapGlyphForge.MAUI.Platforms.Android.Services;
#else
using CheapGlyphForge.MAUI.Services;
#endif

namespace CheapGlyphForge.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor services
        builder.Services.AddMauiBlazorWebView();

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Register Glyph services based on platform
        RegisterGlyphServices(builder.Services);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterGlyphServices(IServiceCollection services)
    {
#if ANDROID
        // Register Android implementation for real device control
        services.AddSingleton<IGlyphInterfaceService, AndroidInterfaceService>();
        services.AddSingleton<IGlyphMatrixService, AndroidMatrixService>();
#else
        // Register simulator services for cross-platform development
        services.AddSingleton<IGlyphInterfaceService, SimulatorInterfaceService>();
        services.AddSingleton<IGlyphMatrixService, SimulatorMatrixService>();
#endif
    }
}