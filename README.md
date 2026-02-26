# Forge.Sdk

C# SDK for the [Forge](https://github.com/centrixsystems/forge) rendering engine. Converts HTML/CSS to PDF, PNG, and other formats via a running Forge server.

## Installation

```sh
dotnet add package Forge.Sdk
```

## Quick Start

```csharp
using Forge.Sdk;

using var client = new ForgeClient("http://localhost:3000");

byte[] pdf = await client.RenderHtml("<h1>Invoice #1234</h1>")
    .Format(OutputFormat.Pdf)
    .Paper("a4")
    .SendAsync();

File.WriteAllBytes("invoice.pdf", pdf);
```

## Usage

### Render URL to PNG

```csharp
byte[] png = await client.RenderUrl("https://example.com")
    .Format(OutputFormat.Png)
    .Width(1280)
    .Height(800)
    .SendAsync();
```

### Color Quantization

```csharp
byte[] eink = await client.RenderHtml("<h1>Dashboard</h1>")
    .Format(OutputFormat.Png)
    .PalettePreset(Palette.Eink)
    .Dither(DitherMethod.FloydSteinberg)
    .SendAsync();
```

### Health Check

```csharp
bool healthy = await client.HealthAsync();
```

## API Reference

### Types

- `OutputFormat`: Pdf, Png, Jpeg, Bmp, Tga, Qoi, Svg
- `Orientation`: Portrait, Landscape
- `Flow`: Auto, Paginate, Continuous
- `DitherMethod`: None, FloydSteinberg, Atkinson, Ordered
- `Palette`: Auto, BlackWhite, Grayscale, Eink

### Errors

- `ForgeException` — base exception
- `ForgeServerException` — 4xx/5xx (has `StatusCode`)
- `ForgeConnectionException` — network failures

## License

MIT
