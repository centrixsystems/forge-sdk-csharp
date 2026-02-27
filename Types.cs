namespace Forge.Sdk;

/// <summary>Output format for rendered content.</summary>
public enum OutputFormat
{
    Pdf,
    Png,
    Jpeg,
    Bmp,
    Tga,
    Qoi,
    Svg,
}

/// <summary>Page orientation.</summary>
public enum Orientation
{
    Portrait,
    Landscape,
}

/// <summary>Document flow mode.</summary>
public enum Flow
{
    Auto,
    Paginate,
    Continuous,
}

/// <summary>Dithering algorithm for color quantization.</summary>
public enum DitherMethod
{
    None,
    FloydSteinberg,
    Atkinson,
    Ordered,
}

/// <summary>Built-in color palette presets.</summary>
public enum Palette
{
    Auto,
    BlackWhite,
    Grayscale,
    Eink,
}

/// <summary>Watermark layer position.</summary>
public enum WatermarkLayer
{
    Over,
    Under,
}

/// <summary>PDF standard compliance level.</summary>
public enum PdfStandard
{
    None,
    A2B,
    A3B,
}

/// <summary>Relationship of an embedded file to the PDF document.</summary>
public enum EmbedRelationship
{
    Alternative,
    Supplement,
    Data,
    Source,
    Unspecified,
}

/// <summary>Barcode symbology type.</summary>
public enum BarcodeType
{
    Qr,
    Code128,
    Ean13,
    UpcA,
    Code39,
}

/// <summary>Barcode anchor position on the page.</summary>
public enum BarcodeAnchor
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}

/// <summary>PDF rendering mode.</summary>
public enum PdfMode
{
    Auto,
    Vector,
    Raster,
}

/// <summary>PDF accessibility compliance level.</summary>
public enum AccessibilityLevel
{
    None,
    Basic,
    PdfUa1,
}

internal static class EnumExtensions
{
    public static string ToApiString(this OutputFormat f) => f switch
    {
        OutputFormat.Pdf => "pdf",
        OutputFormat.Png => "png",
        OutputFormat.Jpeg => "jpeg",
        OutputFormat.Bmp => "bmp",
        OutputFormat.Tga => "tga",
        OutputFormat.Qoi => "qoi",
        OutputFormat.Svg => "svg",
        _ => throw new ArgumentOutOfRangeException(nameof(f)),
    };

    public static string ToApiString(this Orientation o) => o switch
    {
        Orientation.Portrait => "portrait",
        Orientation.Landscape => "landscape",
        _ => throw new ArgumentOutOfRangeException(nameof(o)),
    };

    public static string ToApiString(this Flow f) => f switch
    {
        Flow.Auto => "auto",
        Flow.Paginate => "paginate",
        Flow.Continuous => "continuous",
        _ => throw new ArgumentOutOfRangeException(nameof(f)),
    };

    public static string ToApiString(this DitherMethod d) => d switch
    {
        DitherMethod.None => "none",
        DitherMethod.FloydSteinberg => "floyd-steinberg",
        DitherMethod.Atkinson => "atkinson",
        DitherMethod.Ordered => "ordered",
        _ => throw new ArgumentOutOfRangeException(nameof(d)),
    };

    public static string ToApiString(this Palette p) => p switch
    {
        Palette.Auto => "auto",
        Palette.BlackWhite => "bw",
        Palette.Grayscale => "grayscale",
        Palette.Eink => "eink",
        _ => throw new ArgumentOutOfRangeException(nameof(p)),
    };

    public static string ToApiString(this WatermarkLayer l) => l switch
    {
        WatermarkLayer.Over => "over",
        WatermarkLayer.Under => "under",
        _ => throw new ArgumentOutOfRangeException(nameof(l)),
    };

    public static string ToApiString(this PdfStandard s) => s switch
    {
        PdfStandard.None => "none",
        PdfStandard.A2B => "pdf/a-2b",
        PdfStandard.A3B => "pdf/a-3b",
        _ => throw new ArgumentOutOfRangeException(nameof(s)),
    };

    public static string ToApiString(this EmbedRelationship r) => r switch
    {
        EmbedRelationship.Alternative => "alternative",
        EmbedRelationship.Supplement => "supplement",
        EmbedRelationship.Data => "data",
        EmbedRelationship.Source => "source",
        EmbedRelationship.Unspecified => "unspecified",
        _ => throw new ArgumentOutOfRangeException(nameof(r)),
    };

    public static string ToApiString(this BarcodeType t) => t switch
    {
        BarcodeType.Qr => "qr",
        BarcodeType.Code128 => "code128",
        BarcodeType.Ean13 => "ean13",
        BarcodeType.UpcA => "upca",
        BarcodeType.Code39 => "code39",
        _ => throw new ArgumentOutOfRangeException(nameof(t)),
    };

    public static string ToApiString(this BarcodeAnchor a) => a switch
    {
        BarcodeAnchor.TopLeft => "top-left",
        BarcodeAnchor.TopRight => "top-right",
        BarcodeAnchor.BottomLeft => "bottom-left",
        BarcodeAnchor.BottomRight => "bottom-right",
        _ => throw new ArgumentOutOfRangeException(nameof(a)),
    };

    public static string ToApiString(this PdfMode m) => m switch
    {
        PdfMode.Auto => "auto",
        PdfMode.Vector => "vector",
        PdfMode.Raster => "raster",
        _ => throw new ArgumentOutOfRangeException(nameof(m)),
    };

    public static string ToApiString(this AccessibilityLevel l) => l switch
    {
        AccessibilityLevel.None => "none",
        AccessibilityLevel.Basic => "basic",
        AccessibilityLevel.PdfUa1 => "pdf/ua-1",
        _ => throw new ArgumentOutOfRangeException(nameof(l)),
    };
}
