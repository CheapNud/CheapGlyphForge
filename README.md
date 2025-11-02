# CheapGlyphForge

A .NET 10 MAUI Blazor application for Nothing Phone Glyph LED control and development.

## 🚧 Work in Progress

**⚠️ Pull requests are not being accepted at this time.**

## Overview

CheapGlyphForge provides C# wrappers around Nothing Phone Glyph SDKs, supporting both Glyph Interface (individual LED zones) and Glyph Matrix (25x25 LED grid).

### Features

- Glyph Interface & Matrix control with async/await APIs
- Cross-platform MAUI Blazor app with MudBlazor UI
- Hardware simulator for development without physical devices
- Real-time visualization and pattern designer

## Device Support

| Device | Device ID | Glyph Interface | Glyph Matrix | Zones |
|--------|-----------|-----------------|--------------|-------|
| Phone (1) | 20111 | ✅ | ❌ | A, B, C, D (4 zones) |
| Phone (2) | 22111 | ✅ | ❌ | A, B, C, D, E (5 zones) |
| Phone (2a) | 23111 | ✅ | ❌ | A, B, C (3 zones) |
| Phone (2a) Plus | 23113 | ✅ | ❌ | A, B, C (3 zones) |
| Phone (3) | 24111 | ❌ | ✅ | 25x25 matrix only |

## Technology Stack

- .NET 10.0 RC2 with C# 13
- MAUI + Blazor Server + MudBlazor 8.13.0
- Android API 36 (34+), Windows 10.0.17763.0+, iOS/macOS 15.0+ (simulator)

## Project Structure

- **Core** - Platform-agnostic interfaces (`IGlyphInterfaceService`, `IGlyphMatrixService`) and models
- **MAUI** - Blazor app with simulator and Android native service implementations
- **Bindings** - Android Java bindings for Glyph Interface and Matrix SDKs

## Quick Start

```csharp
// Glyph Interface (individual zones)
await glyphService.Initialize();
await glyphService.ToggleChannel(GlyphChannel.A, intensity: 255);
await glyphService.AnimateChannel(GlyphChannel.B, period: 1000, cycles: 5);

// Glyph Matrix (25x25 grid)
await matrixService.SetPixel(x: 12, y: 12, intensity: 255);
await matrixService.DrawShape(MatrixShape.Circle, centerX: 12, centerY: 12, size: 10);
```

## Simulator Features

- **Glyph Designer** - Device-specific LED visualizations with intensity controls and quick patterns
- **Matrix Playground** - 25x25 grid with click-to-edit pixels and shape generation
- **Combined Preview** - Sequence playback with timing controls

## Requirements

- .NET 10.0 SDK (RC2 build 10.0.100-rc.2.25502.107+)
- Visual Studio 2022 17.12+ or Rider 2024.3+
- Workloads: `dotnet workload install android maui-android maui-windows`

## Building

```bash
git clone https://github.com/CheapNud/CheapGlyphForge.git
cd CheapGlyphForge
dotnet build CheapGlyphForge.MAUI/CheapGlyphForge.MAUI.csproj -f net10.0-android36.0 -c Release
```

## CI/CD

![Build Status](https://github.com/CheapNud/CheapGlyphForge/actions/workflows/dotnet.yml/badge.svg)

GitHub Actions builds Android (APK format), Windows, and iOS/macOS (currently disabled).

## Known Issues

- .NET 10 RC2 + Android API 36 bundletool error (using APK format workaround)
- Device detection placeholder implementation
- Matrix visualization at 35% scale in top-right quadrant

## Architecture

- Interface-based design with dependency injection
- Platform abstraction: Android uses native bindings, others use simulator
- Java bindings wrap Nothing SDKs with C# Task-based async patterns
- Blazor + MudBlazor UI with code-behind and conditional compilation

## License

This project is not affiliated with Nothing Technology Limited.

The Nothing Phone Glyph SDKs are proprietary to Nothing Technology Limited.

---

**Repository**: https://github.com/CheapNud/CheapGlyphForge
**Author**: CheapNud
