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
}
