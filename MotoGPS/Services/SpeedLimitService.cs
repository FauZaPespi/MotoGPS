using System.Globalization;
using System.Text.Json;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class SpeedLimitService : ISpeedLimitService
{
    private const int DefaultLimitKmh = 50;
    private const string OverpassEndpoint = "https://overpass-api.de/api/interpreter";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(10);

    private readonly HttpClient _http;
    private CachedLimit? _cache;

    public SpeedLimitService(HttpClient http) => _http = http;

    public async Task<int> GetLimitAsync(GeoPoint point, CancellationToken ct = default)
    {
        if (_cache is { } cached &&
            DateTimeOffset.UtcNow - cached.At < CacheTtl &&
            cached.NearPoint.DistanceMetersTo(point) < 30)
        {
            return cached.LimitKmh;
        }

        var lat = point.Latitude.ToString("F6", CultureInfo.InvariantCulture);
        var lon = point.Longitude.ToString("F6", CultureInfo.InvariantCulture);
        var query = $"[out:json][timeout:5];way(around:50,{lat},{lon})[highway][maxspeed];out tags 1;";

        try
        {
            using var resp = await _http.PostAsync(
                OverpassEndpoint,
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", query) }),
                ct);

            if (!resp.IsSuccessStatusCode) return _cache?.LimitKmh ?? DefaultLimitKmh;

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (!doc.RootElement.TryGetProperty("elements", out var elements) ||
                elements.ValueKind != JsonValueKind.Array)
            {
                return _cache?.LimitKmh ?? DefaultLimitKmh;
            }

            foreach (var element in elements.EnumerateArray())
            {
                if (!element.TryGetProperty("tags", out var tags)) continue;
                if (!tags.TryGetProperty("maxspeed", out var maxspeed)) continue;

                var raw = maxspeed.GetString();
                if (TryParseMaxspeed(raw, out var limit))
                {
                    _cache = new CachedLimit(point, limit, DateTimeOffset.UtcNow);
                    return limit;
                }
            }
        }
        catch
        {
            // Network or parse failure — fall through to defaults.
        }

        return _cache?.LimitKmh ?? DefaultLimitKmh;
    }

    private static bool TryParseMaxspeed(string? raw, out int kmh)
    {
        kmh = 0;
        if (string.IsNullOrWhiteSpace(raw)) return false;

        var trimmed = raw.Trim();

        if (trimmed.EndsWith("mph", StringComparison.OrdinalIgnoreCase))
        {
            var digits = new string(trimmed.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var mph))
            {
                kmh = (int)Math.Round(mph * 1.609344);
                return true;
            }
            return false;
        }

        var numericPart = new string(trimmed.TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(numericPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out kmh);
    }

    private sealed record CachedLimit(GeoPoint NearPoint, int LimitKmh, DateTimeOffset At);
}
