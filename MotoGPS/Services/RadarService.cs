using System.Globalization;
using System.Text.Json;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class RadarService : IRadarService
{
    private const string OverpassEndpoint = "https://overpass-api.de/api/interpreter";
    private const int QueryRadiusM = 600;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly HttpClient _http;
    private CacheEntry? _cache;

    public RadarService(HttpClient http) => _http = http;

    public async Task<RadarProximity> GetClosestAsync(GeoPoint point, CancellationToken ct = default)
    {
        if (_cache is { } cached &&
            DateTimeOffset.UtcNow - cached.At < CacheTtl &&
            cached.Anchor.DistanceMetersTo(point) < 50)
        {
            return ClosestFrom(cached.Cameras, point);
        }

        var lat = point.Latitude.ToString("F6", CultureInfo.InvariantCulture);
        var lon = point.Longitude.ToString("F6", CultureInfo.InvariantCulture);
        var query = $"[out:json][timeout:5];node(around:{QueryRadiusM},{lat},{lon})[highway=speed_camera];out;";

        try
        {
            using var resp = await _http.PostAsync(
                OverpassEndpoint,
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", query) }),
                ct);

            if (!resp.IsSuccessStatusCode)
            {
                return _cache is null ? new RadarProximity(null, _empty) : ClosestFrom(_cache.Cameras, point);
            }

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var cameras = new List<GeoPoint>();

            if (doc.RootElement.TryGetProperty("elements", out var elements) &&
                elements.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in elements.EnumerateArray())
                {
                    if (element.TryGetProperty("lat", out var latProp) &&
                        element.TryGetProperty("lon", out var lonProp))
                    {
                        cameras.Add(new GeoPoint(latProp.GetDouble(), lonProp.GetDouble()));
                    }
                }
            }

            _cache = new CacheEntry(point, cameras, DateTimeOffset.UtcNow);
            return ClosestFrom(cameras, point);
        }
        catch
        {
            return _cache is null ? new RadarProximity(null, _empty) : ClosestFrom(_cache.Cameras, point);
        }
    }

    private static readonly IReadOnlyList<GeoPoint> _empty = Array.Empty<GeoPoint>();

    private static RadarProximity ClosestFrom(IReadOnlyList<GeoPoint> cameras, GeoPoint from)
    {
        if (cameras.Count == 0) return new RadarProximity(null, _empty);

        double? best = null;
        foreach (var camera in cameras)
        {
            var d = camera.DistanceMetersTo(from);
            if (best is null || d < best) best = d;
        }
        return new RadarProximity(best, cameras);
    }

    private sealed record CacheEntry(GeoPoint Anchor, IReadOnlyList<GeoPoint> Cameras, DateTimeOffset At);
}
