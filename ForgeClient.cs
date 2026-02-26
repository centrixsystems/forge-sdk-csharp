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

        return payload;
    }

    /// <summary>Send the render request.</summary>
    public Task<byte[]> SendAsync(CancellationToken ct = default) =>
        _client.SendAsync(BuildPayload(), ct);
}
