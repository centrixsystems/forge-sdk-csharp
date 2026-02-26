# Forge.Sdk

C# SDK for the [Forge](https://github.com/centrixsystems/forge) rendering engine. Converts HTML/CSS to PDF, PNG, and other formats via a running Forge server.

Targets .NET 8.0. Uses `HttpClient` and `System.Text.Json`.

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

### Render HTML to PDF

```csharp
byte[] pdf = await client.RenderHtml("<h1>Hello</h1>")
    .Format(OutputFormat.Pdf)
    .Paper("a4")
    .Orientation(Orientation.Portrait)
    .Margins("25.4,25.4,25.4,25.4")
    .Flow(Flow.Paginate)
    .SendAsync();
```

### Render URL to PNG

```csharp
byte[] png = await client.RenderUrl("https://example.com")
    .Format(OutputFormat.Png)
    .Width(1280)
    .Height(800)
    .Density(2.0)
    .SendAsync();
```

### Color Quantization

Reduce colors for e-ink displays or limited-palette output.

```csharp
byte[] eink = await client.RenderHtml("<h1>Dashboard</h1>")
    .Format(OutputFormat.Png)
    .PalettePreset(Palette.Eink)
    .Dither(DitherMethod.FloydSteinberg)
    .SendAsync();
```

### Custom Palette

```csharp
byte[] img = await client.RenderHtml("<h1>Brand</h1>")
    .Format(OutputFormat.Png)
    .CustomPalette(new[] { "#000000", "#ffffff", "#ff0000" })
    .Dither(DitherMethod.Atkinson)
    .SendAsync();
```

### PDF Metadata

Set PDF document metadata and enable bookmarks.

```csharp
byte[] pdf = await client.RenderHtml("<h1>Report</h1>")
    .Format(OutputFormat.Pdf)
    .Paper("a4")
    .PdfTitle("Quarterly Report")
    .PdfAuthor("Centrix ERP")
    .PdfSubject("Q4 2025 Financial Summary")
    .PdfKeywords("report,finance,quarterly")
    .PdfCreator("Forge Renderer")
    .PdfBookmarks(true)
    .SendAsync();
```

### Cancellation

All async methods accept an optional `CancellationToken`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

byte[] pdf = await client.RenderHtml("<h1>Slow render</h1>")
    .Format(OutputFormat.Pdf)
    .SendAsync(cts.Token);
```

### Custom HttpClient

```csharp
var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
using var client = new ForgeClient("http://forge:3000", httpClient);
```

### Health Check

```csharp
bool healthy = await client.HealthAsync();
```

## API Reference

### `ForgeClient`

```csharp
new ForgeClient(string baseUrl, HttpClient? httpClient = null)
```

Implements `IDisposable`. Use `using` for automatic cleanup.

| Method | Returns | Description |
|--------|---------|-------------|
| `RenderHtml(html)` | `RenderRequestBuilder` | Start a render request from HTML |
| `RenderUrl(url)` | `RenderRequestBuilder` | Start a render request from a URL |
| `HealthAsync(ct)` | `Task<bool>` | Check server health |

### `RenderRequestBuilder`

All methods return the builder for chaining. Call `.SendAsync()` to execute.

| Method | Type | Description |
|--------|------|-------------|
| `Format` | `OutputFormat` | Output format (default: `Pdf`) |
| `Width` | `int` | Viewport width in CSS pixels |
| `Height` | `int` | Viewport height in CSS pixels |
| `Paper` | `string` | Paper size: a3, a4, a5, b4, b5, letter, legal, ledger |
| `Orientation` | `Orientation` | `Portrait` or `Landscape` |
| `Margins` | `string` | Preset (`default`, `none`, `narrow`) or `"T,R,B,L"` in mm |
| `Flow` | `Flow` | `Auto`, `Paginate`, or `Continuous` |
| `Density` | `double` | Output DPI (default: 96) |
| `Background` | `string` | CSS background color (e.g. `"#ffffff"`) |
| `Timeout` | `int` | Page load timeout in seconds |
| `Colors` | `int` | Quantization color count (2-256) |
| `PalettePreset` | `Palette` | Built-in palette preset |
| `CustomPalette` | `string[]` | Array of hex color strings |
| `Dither` | `DitherMethod` | Dithering algorithm |
| `PdfTitle` | `string` | PDF document title |
| `PdfAuthor` | `string` | PDF document author |
| `PdfSubject` | `string` | PDF document subject |
| `PdfKeywords` | `string` | Comma-separated PDF keywords |
| `PdfCreator` | `string` | PDF creator application name |
| `PdfBookmarks` | `bool` | Enable PDF bookmarks from headings |

| Terminal Method | Returns | Description |
|-----------------|---------|-------------|
| `SendAsync(ct)` | `Task<byte[]>` | Execute the render request |

### Enums

| Enum | Values |
|------|--------|
| `OutputFormat` | `Pdf`, `Png`, `Jpeg`, `Bmp`, `Tga`, `Qoi`, `Svg` |
| `Orientation` | `Portrait`, `Landscape` |
| `Flow` | `Auto`, `Paginate`, `Continuous` |
| `DitherMethod` | `None`, `FloydSteinberg`, `Atkinson`, `Ordered` |
| `Palette` | `Auto`, `BlackWhite`, `Grayscale`, `Eink` |

### Exceptions

| Exception | Properties | Description |
|-----------|------------|-------------|
| `ForgeException` | `Message` | Base exception for all SDK errors |
| `ForgeServerException` | `StatusCode: int` | Server returned 4xx/5xx |
| `ForgeConnectionException` | `InnerException` | Network failure |

## Requirements

- .NET 8.0+
- A running [Forge](https://github.com/centrixsystems/forge) server

## License

MIT
