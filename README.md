# CheapGlyphForge

A unified Blazor + .NET Android app for Nothing Phone Glyph control.

## 🚧 Work in Progress

This project is in active development and not ready for public use.

**⚠️ Pull requests are not being accepted at this time.**

## What is this?

CheapGlyphForge provides clean C# wrapper interfaces for the Nothing Phone Glyph SDK, enabling:

- **Glyph Interface Control** - Individual light control and animations
- **Glyph Matrix Control** - 25x25 LED matrix for Phone (3) models  
- **Cross-platform Development** - MAUI Blazor app with Android Glyph Toy support
- **Modern C# APIs** - Async/await patterns with proper error handling

## Current Approach

Currently using the **Matrix SDK only** (which includes Interface functionality) for simplicity. 

**Future plans:** Split into lightweight Interface-only and full Matrix packages for broader ecosystem support.

## Requirements

- Nothing Phone with Glyph support
- Android 14+ (API 34)
- .NET 9.0

## Architecture

```
├── CheapGlyphForge.Core          # Platform-agnostic interfaces & models
├── CheapGlyphForge.MAUI          # Cross-platform Blazor app  
└── CheapGlyphForge.GlyphMatrix.Binding  # Nothing SDK bindings
```

---

*This project is not affiliated with Nothing Technology Limited.*
