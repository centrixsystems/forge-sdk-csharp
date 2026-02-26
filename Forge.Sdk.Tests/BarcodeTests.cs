using System.Text.Json.Nodes;
using Xunit;

namespace Forge.Sdk.Tests;

public class BarcodeTests
{
    private static ForgeClient MakeClient() => new("http://localhost:8080");

    [Fact]
    public void SingleBarcodeMinimalPayload()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(BarcodeType.Qr, "https://example.com")
            .BuildPayload();

        var pdf = payload["pdf"]!.AsObject();
        var barcodes = pdf["barcodes"]!.AsArray();
        Assert.Single(barcodes);

        var bc = barcodes[0]!.AsObject();
        Assert.Equal("qr", bc["type"]!.GetValue<string>());
        Assert.Equal("https://example.com", bc["data"]!.GetValue<string>());
        Assert.Null(bc["x"]);
        Assert.Null(bc["y"]);
        Assert.Null(bc["width"]);
        Assert.Null(bc["height"]);
        Assert.Null(bc["anchor"]);
        Assert.Null(bc["foreground"]);
        Assert.Null(bc["background"]);
        Assert.Null(bc["draw_background"]);
        Assert.Null(bc["pages"]);
    }

    [Fact]
    public void BarcodeWithAllOptions()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(
                BarcodeType.Code128, "ABC-123",
                x: 10.5, y: 20.0, width: 100.0, height: 50.0,
                anchor: BarcodeAnchor.TopRight,
                foreground: "#000000", background: "#FFFFFF",
                drawBackground: true, pages: "1,3-5")
            .BuildPayload();

        var pdf = payload["pdf"]!.AsObject();
        var bc = pdf["barcodes"]!.AsArray()[0]!.AsObject();
        Assert.Equal("code128", bc["type"]!.GetValue<string>());
        Assert.Equal("ABC-123", bc["data"]!.GetValue<string>());
        Assert.Equal(10.5, bc["x"]!.GetValue<double>());
        Assert.Equal(20.0, bc["y"]!.GetValue<double>());
        Assert.Equal(100.0, bc["width"]!.GetValue<double>());
        Assert.Equal(50.0, bc["height"]!.GetValue<double>());
        Assert.Equal("top-right", bc["anchor"]!.GetValue<string>());
        Assert.Equal("#000000", bc["foreground"]!.GetValue<string>());
        Assert.Equal("#FFFFFF", bc["background"]!.GetValue<string>());
        Assert.True(bc["draw_background"]!.GetValue<bool>());
        Assert.Equal("1,3-5", bc["pages"]!.GetValue<string>());
    }

    [Fact]
    public void MultipleBarcodes()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(BarcodeType.Qr, "first")
            .PdfBarcode(BarcodeType.Ean13, "5901234123457")
            .PdfBarcode(BarcodeType.Code39, "HELLO")
            .BuildPayload();

        var barcodes = payload["pdf"]!["barcodes"]!.AsArray();
        Assert.Equal(3, barcodes.Count);
        Assert.Equal("qr", barcodes[0]!["type"]!.GetValue<string>());
        Assert.Equal("first", barcodes[0]!["data"]!.GetValue<string>());
        Assert.Equal("ean13", barcodes[1]!["type"]!.GetValue<string>());
        Assert.Equal("5901234123457", barcodes[1]!["data"]!.GetValue<string>());
        Assert.Equal("code39", barcodes[2]!["type"]!.GetValue<string>());
        Assert.Equal("HELLO", barcodes[2]!["data"]!.GetValue<string>());
    }

    [Fact]
    public void BarcodeWithAnchorVariants()
    {
        var client = MakeClient();

        var anchors = new (BarcodeAnchor anchor, string expected)[]
        {
            (BarcodeAnchor.TopLeft, "top-left"),
            (BarcodeAnchor.TopRight, "top-right"),
            (BarcodeAnchor.BottomLeft, "bottom-left"),
            (BarcodeAnchor.BottomRight, "bottom-right"),
        };

        foreach (var (anchor, expected) in anchors)
        {
            var payload = client.RenderHtml("<h1>Test</h1>")
                .PdfBarcode(BarcodeType.Qr, "test", anchor: anchor)
                .BuildPayload();

            var bc = payload["pdf"]!["barcodes"]!.AsArray()[0]!.AsObject();
            Assert.Equal(expected, bc["anchor"]!.GetValue<string>());
        }
    }

    [Theory]
    [InlineData(BarcodeType.Qr, "qr")]
    [InlineData(BarcodeType.Code128, "code128")]
    [InlineData(BarcodeType.Ean13, "ean13")]
    [InlineData(BarcodeType.UpcA, "upca")]
    [InlineData(BarcodeType.Code39, "code39")]
    public void BarcodeTypeSerialization(BarcodeType type, string expected)
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(type, "data")
            .BuildPayload();

        var bc = payload["pdf"]!["barcodes"]!.AsArray()[0]!.AsObject();
        Assert.Equal(expected, bc["type"]!.GetValue<string>());
    }

    [Fact]
    public void NoBarcodesMeansNoPdfSection()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .BuildPayload();

        Assert.Null(payload["pdf"]);
    }

    [Fact]
    public void BarcodeDrawBackgroundFalse()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(BarcodeType.Qr, "test", drawBackground: false)
            .BuildPayload();

        var bc = payload["pdf"]!["barcodes"]!.AsArray()[0]!.AsObject();
        Assert.False(bc["draw_background"]!.GetValue<bool>());
    }

    [Fact]
    public void BarcodeWithUpcA()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfBarcode(BarcodeType.UpcA, "012345678905", anchor: BarcodeAnchor.BottomLeft)
            .BuildPayload();

        var bc = payload["pdf"]!["barcodes"]!.AsArray()[0]!.AsObject();
        Assert.Equal("upca", bc["type"]!.GetValue<string>());
        Assert.Equal("bottom-left", bc["anchor"]!.GetValue<string>());
    }
}

public class WatermarkPagesTests
{
    private static ForgeClient MakeClient() => new("http://localhost:8080");

    [Fact]
    public void WatermarkWithPages()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfWatermarkText("DRAFT")
            .PdfWatermarkPages("1,3-5")
            .BuildPayload();

        var wm = payload["pdf"]!["watermark"]!.AsObject();
        Assert.Equal("DRAFT", wm["text"]!.GetValue<string>());
        Assert.Equal("1,3-5", wm["pages"]!.GetValue<string>());
    }

    [Fact]
    public void WatermarkPagesOnlyCreatesWatermarkObject()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfWatermarkPages("2-4")
            .BuildPayload();

        var wm = payload["pdf"]!["watermark"]!.AsObject();
        Assert.Equal("2-4", wm["pages"]!.GetValue<string>());
    }

    [Fact]
    public void WatermarkWithoutPagesHasNoPagesField()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfWatermarkText("DRAFT")
            .BuildPayload();

        var wm = payload["pdf"]!["watermark"]!.AsObject();
        Assert.Null(wm["pages"]);
    }

    [Fact]
    public void BarcodeAndWatermarkCoexist()
    {
        var client = MakeClient();
        var payload = client.RenderHtml("<h1>Test</h1>")
            .PdfWatermarkText("CONFIDENTIAL")
            .PdfWatermarkPages("1")
            .PdfBarcode(BarcodeType.Qr, "https://example.com", pages: "2-3")
            .BuildPayload();

        var pdf = payload["pdf"]!.AsObject();
        var wm = pdf["watermark"]!.AsObject();
        Assert.Equal("CONFIDENTIAL", wm["text"]!.GetValue<string>());
        Assert.Equal("1", wm["pages"]!.GetValue<string>());

        var barcodes = pdf["barcodes"]!.AsArray();
        Assert.Single(barcodes);
        Assert.Equal("qr", barcodes[0]!["type"]!.GetValue<string>());
        Assert.Equal("2-3", barcodes[0]!["pages"]!.GetValue<string>());
    }
}
