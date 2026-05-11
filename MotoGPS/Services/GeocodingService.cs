using System.Globalization;
using System.Text.Json;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class GeocodingService : IGeocodingService
{
    private const string NominatimUrl = "https://nominatim.openstreetmap.org/search";
    private readonly HttpClient _http;

    public GeocodingService(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<GeocodingResult>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];

        var encoded = Uri.EscapeDataString(query.Trim());
        var url = $"{NominatimUrl}?q={encoded}&format=json&limit=5&addressdetails=0";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("User-Agent", "MotoGPS/1.0 (fauzadev@proton.me)");

        try
        {
            using var resp = await _http.SendAsync(request, ct);
            if (!resp.IsSuccessStatusCode) return [];

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (doc.RootElement.ValueKind != JsonValueKind.Array) return [];

            var results = new List<GeocodingResult>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                var name = item.TryGetProperty("display_name", out var n) ? n.GetString() : null;
                var latStr = item.TryGetProperty("lat", out var la) ? la.GetString() : null;
                var lonStr = item.TryGetProperty("lon", out var lo) ? lo.GetString() : null;

                if (name is null || latStr is null || lonStr is null) continue;
                if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat)) continue;
                if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)) continue;

                results.Add(new GeocodingResult(name, new GeoPoint(lat, lon)));
            }

            return results;
        }
        catch
        {
            return [];
        }
    }
}
