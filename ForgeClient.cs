using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Forge.Sdk;

/// <summary>Client for a Forge rendering server.</summary>
public class ForgeClient : IDisposable
{
    private readonly string _baseUrl;
    private readonly HttpClient _http;

    public ForgeClient(string baseUrl, HttpClient? httpClient = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
    }

    /// <summary>Start a render request from an HTML string.</summary>
    public RenderRequestBuilder RenderHtml(string html) =>
        new(this, html: html);

    /// <summary>Start a render request from a URL.</summary>
    public RenderRequestBuilder RenderUrl(string url) =>
        new(this, url: url);

    /// <summary>Check if the server is healthy.</summary>
    public async Task<bool> HealthAsync(CancellationToken ct = default)
    {
        try
        {
            var resp = await _http.GetAsync($"{_baseUrl}/health", ct);
            return resp.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    internal async Task<byte[]> SendAsync(JsonObject payload, CancellationToken ct)
    {
        HttpResponseMessage resp;
        try
        {
            var content = new StringContent(
                payload.ToJsonString(),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            resp = await _http.PostAsync($"{_baseUrl}/render", content, ct);
        }
        catch (HttpRequestException ex)
        {
            throw new ForgeConnectionException(ex);
        }

        if (!resp.IsSuccessStatusCode)
        {
            string message;
            try
            {
                var body = await resp.Content.ReadFromJsonAsync<JsonObject>(ct);
                message = body?["error"]?.GetValue<string>() ?? $"HTTP {(int)resp.StatusCode}";
            }
            catch
            {
                message = $"HTTP {(int)resp.StatusCode}";
            }
            throw new ForgeServerException((int)resp.StatusCode, message);
        }

        return await resp.Content.ReadAsByteArrayAsync(ct);
    }

    public void Dispose()
    {
        _http.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>Builder for a render request.</summary>
public class RenderRequestBuilder
{
    private readonly ForgeClient _client;
    private readonly string? _html;
    private readonly string? _url;
    private OutputFormat _format = OutputFormat.Pdf;
    private int? _width;
    private int? _height;
    private string? _paper;
    private Orientation? _orientation;
    private string? _margins;
    private Flow? _flow;
    private double? _density;
    private string? _background;
    private int? _timeout;
    private int? _colors;
    private object? _palette; // Palette enum or string[]
    private DitherMethod? _dither;
    private string? _pdfTitle;
    private string? _pdfAuthor;
    private string? _pdfSubject;
    private string? _pdfKeywords;
    private string? _pdfCreator;
    private bool? _pdfBookmarks;
    private bool? _pdfPageNumbers;
    private string? _pdfWatermarkText;
    private string? _pdfWatermarkImage; // base64-encoded
    private float? _pdfWatermarkOpacity;
    private float? _pdfWatermarkRotation;
    private string? _pdfWatermarkColor;
    private float? _pdfWatermarkFontSize;
    private float? _pdfWatermarkScale;
    private WatermarkLayer? _pdfWatermarkLayer;
    private string? _pdfWatermarkPages;
    private PdfStandard? _pdfStandard;
    private List<(string path, string data, string? mimeType, string? description, EmbedRelationship? relationship)>? _pdfEmbeddedFiles;
    private List<(BarcodeType type, string data, double? x, double? y, double? width, double? height, BarcodeAnchor? anchor, string? foreground, string? background, bool? drawBackground, string? pages)> _pdfBarcodes = new();
    private PdfMode? _pdfMode;
    private string? _pdfSignCertificate;
    private string? _pdfSignPassword;
    private string? _pdfSignName;
    private string? _pdfSignReason;
    private string? _pdfSignLocation;
    private string? _pdfSignTimestampUrl;
    private string? _pdfUserPassword;
    private string? _pdfOwnerPassword;
    private string? _pdfPermissions;
    private AccessibilityLevel? _pdfAccessibility;
    private bool? _pdfLinearize;

    internal RenderRequestBuilder(ForgeClient client, string? html = null, string? url = null)
    {
        _client = client;
        _html = html;
        _url = url;
    }

    public RenderRequestBuilder Format(OutputFormat f) { _format = f; return this; }
    public RenderRequestBuilder Width(int px) { _width = px; return this; }
    public RenderRequestBuilder Height(int px) { _height = px; return this; }
    public RenderRequestBuilder Paper(string size) { _paper = size; return this; }
    public RenderRequestBuilder Orientation(Orientation o) { _orientation = o; return this; }
    public RenderRequestBuilder Margins(string m) { _margins = m; return this; }
    public RenderRequestBuilder Flow(Flow f) { _flow = f; return this; }
    public RenderRequestBuilder Density(double dpi) { _density = dpi; return this; }
    public RenderRequestBuilder Background(string color) { _background = color; return this; }
    public RenderRequestBuilder Timeout(int seconds) { _timeout = seconds; return this; }
    public RenderRequestBuilder Colors(int n) { _colors = n; return this; }
    public RenderRequestBuilder PalettePreset(Palette p) { _palette = p; return this; }
    public RenderRequestBuilder CustomPalette(string[] colors) { _palette = colors; return this; }
    public RenderRequestBuilder Dither(DitherMethod method) { _dither = method; return this; }
    public RenderRequestBuilder PdfTitle(string title) { _pdfTitle = title; return this; }
    public RenderRequestBuilder PdfAuthor(string author) { _pdfAuthor = author; return this; }
    public RenderRequestBuilder PdfSubject(string subject) { _pdfSubject = subject; return this; }
    public RenderRequestBuilder PdfKeywords(string keywords) { _pdfKeywords = keywords; return this; }
    public RenderRequestBuilder PdfCreator(string creator) { _pdfCreator = creator; return this; }
    public RenderRequestBuilder PdfBookmarks(bool enabled) { _pdfBookmarks = enabled; return this; }
    public RenderRequestBuilder PdfPageNumbers(bool enabled) { _pdfPageNumbers = enabled; return this; }
    public RenderRequestBuilder PdfWatermarkText(string text) { _pdfWatermarkText = text; return this; }
    public RenderRequestBuilder PdfWatermarkImage(string base64Data) { _pdfWatermarkImage = base64Data; return this; }
    public RenderRequestBuilder PdfWatermarkOpacity(float opacity) { _pdfWatermarkOpacity = opacity; return this; }
    public RenderRequestBuilder PdfWatermarkRotation(float degrees) { _pdfWatermarkRotation = degrees; return this; }
    public RenderRequestBuilder PdfWatermarkColor(string hex) { _pdfWatermarkColor = hex; return this; }
    public RenderRequestBuilder PdfWatermarkFontSize(float size) { _pdfWatermarkFontSize = size; return this; }
    public RenderRequestBuilder PdfWatermarkScale(float scale) { _pdfWatermarkScale = scale; return this; }
    public RenderRequestBuilder PdfWatermarkLayer(WatermarkLayer layer) { _pdfWatermarkLayer = layer; return this; }
    public RenderRequestBuilder PdfWatermarkPages(string pages) { _pdfWatermarkPages = pages; return this; }
    public RenderRequestBuilder PdfStandard(PdfStandard standard) { _pdfStandard = standard; return this; }
    public RenderRequestBuilder PdfBarcode(BarcodeType type, string data, double? x = null, double? y = null, double? width = null, double? height = null, BarcodeAnchor? anchor = null, string? foreground = null, string? background = null, bool? drawBackground = null, string? pages = null)
    {
        _pdfBarcodes.Add((type, data, x, y, width, height, anchor, foreground, background, drawBackground, pages));
        return this;
    }
    public RenderRequestBuilder PdfAttach(string path, string base64Data, string? mimeType = null, string? description = null, EmbedRelationship? relationship = null)
    {
        _pdfEmbeddedFiles ??= new();
        _pdfEmbeddedFiles.Add((path, base64Data, mimeType, description, relationship));
        return this;
    }
    public RenderRequestBuilder PdfMode(PdfMode mode) { _pdfMode = mode; return this; }
    public RenderRequestBuilder PdfSignCertificate(string data) { _pdfSignCertificate = data; return this; }
    public RenderRequestBuilder PdfSignPassword(string password) { _pdfSignPassword = password; return this; }
    public RenderRequestBuilder PdfSignName(string name) { _pdfSignName = name; return this; }
    public RenderRequestBuilder PdfSignReason(string reason) { _pdfSignReason = reason; return this; }
    public RenderRequestBuilder PdfSignLocation(string location) { _pdfSignLocation = location; return this; }
    public RenderRequestBuilder PdfSignTimestampUrl(string url) { _pdfSignTimestampUrl = url; return this; }
    public RenderRequestBuilder PdfUserPassword(string password) { _pdfUserPassword = password; return this; }
    public RenderRequestBuilder PdfOwnerPassword(string password) { _pdfOwnerPassword = password; return this; }
    public RenderRequestBuilder PdfPermissions(string permissions) { _pdfPermissions = permissions; return this; }
    public RenderRequestBuilder PdfAccessibility(AccessibilityLevel level) { _pdfAccessibility = level; return this; }
    public RenderRequestBuilder PdfLinearize(bool enabled) { _pdfLinearize = enabled; return this; }

    /// <summary>Build the JSON payload.</summary>
    public JsonObject BuildPayload()
    {
        var payload = new JsonObject { ["format"] = _format.ToApiString() };

        if (_html != null) payload["html"] = _html;
        if (_url != null) payload["url"] = _url;
        if (_width.HasValue) payload["width"] = _width.Value;
        if (_height.HasValue) payload["height"] = _height.Value;
        if (_paper != null) payload["paper"] = _paper;
        if (_orientation.HasValue) payload["orientation"] = _orientation.Value.ToApiString();
        if (_margins != null) payload["margins"] = _margins;
        if (_flow.HasValue) payload["flow"] = _flow.Value.ToApiString();
        if (_density.HasValue) payload["density"] = _density.Value;
        if (_background != null) payload["background"] = _background;
        if (_timeout.HasValue) payload["timeout"] = _timeout.Value;

        if (_colors.HasValue || _palette != null || _dither.HasValue)
        {
            var q = new JsonObject();
            if (_colors.HasValue) q["colors"] = _colors.Value;
            if (_palette is Palette preset)
                q["palette"] = preset.ToApiString();
            else if (_palette is string[] custom)
            {
                var arr = new JsonArray();
                foreach (var c in custom) arr.Add(c);
                q["palette"] = arr;
            }
            if (_dither.HasValue) q["dither"] = _dither.Value.ToApiString();
            payload["quantize"] = q;
        }

        if (_pdfTitle != null || _pdfAuthor != null || _pdfSubject != null ||
            _pdfKeywords != null || _pdfCreator != null || _pdfBookmarks.HasValue ||
            _pdfPageNumbers.HasValue ||
            _pdfWatermarkText != null || _pdfWatermarkImage != null || _pdfWatermarkOpacity.HasValue ||
            _pdfWatermarkRotation.HasValue || _pdfWatermarkColor != null || _pdfWatermarkFontSize.HasValue ||
            _pdfWatermarkScale.HasValue || _pdfWatermarkLayer.HasValue || _pdfWatermarkPages != null ||
            _pdfStandard.HasValue || _pdfEmbeddedFiles != null || _pdfBarcodes.Count > 0 ||
            _pdfMode.HasValue || _pdfSignCertificate != null || _pdfUserPassword != null ||
            _pdfOwnerPassword != null || _pdfPermissions != null || _pdfAccessibility.HasValue ||
            _pdfLinearize.HasValue)
        {
            var p = new JsonObject();
            if (_pdfTitle != null) p["title"] = _pdfTitle;
            if (_pdfAuthor != null) p["author"] = _pdfAuthor;
            if (_pdfSubject != null) p["subject"] = _pdfSubject;
            if (_pdfKeywords != null) p["keywords"] = _pdfKeywords;
            if (_pdfCreator != null) p["creator"] = _pdfCreator;
            if (_pdfBookmarks.HasValue) p["bookmarks"] = _pdfBookmarks.Value;
            if (_pdfPageNumbers.HasValue) p["page_numbers"] = _pdfPageNumbers.Value;
            if (_pdfStandard.HasValue) p["standard"] = _pdfStandard.Value.ToApiString();
            if (_pdfWatermarkText != null || _pdfWatermarkImage != null || _pdfWatermarkOpacity.HasValue ||
                _pdfWatermarkRotation.HasValue || _pdfWatermarkColor != null || _pdfWatermarkFontSize.HasValue ||
                _pdfWatermarkScale.HasValue || _pdfWatermarkLayer.HasValue || _pdfWatermarkPages != null)
            {
                var wm = new JsonObject();
                if (_pdfWatermarkText != null) wm["text"] = _pdfWatermarkText;
                if (_pdfWatermarkImage != null) wm["image_data"] = _pdfWatermarkImage;
                if (_pdfWatermarkOpacity.HasValue) wm["opacity"] = _pdfWatermarkOpacity.Value;
                if (_pdfWatermarkRotation.HasValue) wm["rotation"] = _pdfWatermarkRotation.Value;
                if (_pdfWatermarkColor != null) wm["color"] = _pdfWatermarkColor;
                if (_pdfWatermarkFontSize.HasValue) wm["font_size"] = _pdfWatermarkFontSize.Value;
                if (_pdfWatermarkScale.HasValue) wm["scale"] = _pdfWatermarkScale.Value;
                if (_pdfWatermarkLayer.HasValue) wm["layer"] = _pdfWatermarkLayer.Value.ToApiString();
                if (_pdfWatermarkPages != null) wm["pages"] = _pdfWatermarkPages;
                p["watermark"] = wm;
            }
            if (_pdfEmbeddedFiles != null)
            {
                var arr = new JsonArray();
                foreach (var (path, data, mimeType, description, relationship) in _pdfEmbeddedFiles)
                {
                    var ef = new JsonObject { ["path"] = path, ["data"] = data };
                    if (mimeType != null) ef["mime_type"] = mimeType;
                    if (description != null) ef["description"] = description;
                    if (relationship.HasValue) ef["relationship"] = relationship.Value.ToApiString();
                    arr.Add(ef);
                }
                p["embedded_files"] = arr;
            }
            if (_pdfBarcodes.Count > 0)
            {
                var arr = new JsonArray();
                foreach (var (type, data, x, y, width, height, anchor, foreground, background, drawBackground, pages) in _pdfBarcodes)
                {
                    var bc = new JsonObject { ["type"] = type.ToApiString(), ["data"] = data };
                    if (x.HasValue) bc["x"] = x.Value;
                    if (y.HasValue) bc["y"] = y.Value;
                    if (width.HasValue) bc["width"] = width.Value;
                    if (height.HasValue) bc["height"] = height.Value;
                    if (anchor.HasValue) bc["anchor"] = anchor.Value.ToApiString();
                    if (foreground != null) bc["foreground"] = foreground;
                    if (background != null) bc["background"] = background;
                    if (drawBackground.HasValue) bc["draw_background"] = drawBackground.Value;
                    if (pages != null) bc["pages"] = pages;
                    arr.Add(bc);
                }
                p["barcodes"] = arr;
            }
            if (_pdfMode.HasValue) p["mode"] = _pdfMode.Value.ToApiString();
            if (_pdfSignCertificate != null)
            {
                var sig = new JsonObject { ["certificate_data"] = _pdfSignCertificate };
                if (_pdfSignPassword != null) sig["password"] = _pdfSignPassword;
                if (_pdfSignName != null) sig["signer_name"] = _pdfSignName;
                if (_pdfSignReason != null) sig["reason"] = _pdfSignReason;
                if (_pdfSignLocation != null) sig["location"] = _pdfSignLocation;
                if (_pdfSignTimestampUrl != null) sig["timestamp_url"] = _pdfSignTimestampUrl;
                p["signature"] = sig;
            }
            if (_pdfUserPassword != null || _pdfOwnerPassword != null || _pdfPermissions != null)
            {
                var enc = new JsonObject();
                if (_pdfUserPassword != null) enc["user_password"] = _pdfUserPassword;
                if (_pdfOwnerPassword != null) enc["owner_password"] = _pdfOwnerPassword;
                if (_pdfPermissions != null) enc["permissions"] = _pdfPermissions;
                p["encryption"] = enc;
            }
            if (_pdfAccessibility.HasValue) p["accessibility"] = _pdfAccessibility.Value.ToApiString();
            if (_pdfLinearize.HasValue) p["linearize"] = _pdfLinearize.Value;
            payload["pdf"] = p;
        }

        return payload;
    }

    /// <summary>Send the render request.</summary>
    public Task<byte[]> SendAsync(CancellationToken ct = default) =>
        _client.SendAsync(BuildPayload(), ct);
}
