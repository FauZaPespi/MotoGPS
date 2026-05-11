using System.Globalization;
using System.Text.Json;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class RoutingService : IRoutingService
{
    private const string BaseUrl = "https://router.project-osrm.org/route/v1/driving";

    private readonly HttpClient _http;

    public RoutingService(HttpClient http) => _http = http;

    public async Task<RouteResult?> CalculateAsync(GeoPoint from, GeoPoint to, CancellationToken ct = default)
    {
        var inv = CultureInfo.InvariantCulture;
        var url = $"{BaseUrl}/" +
                  $"{from.Longitude.ToString("F6", inv)},{from.Latitude.ToString("F6", inv)};" +
                  $"{to.Longitude.ToString("F6", inv)},{to.Latitude.ToString("F6", inv)}" +
                  "?overview=full&geometries=geojson&steps=true";

        try
        {
            using var resp = await _http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode) return null;

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (!doc.RootElement.TryGetProperty("routes", out var routes) ||
                routes.ValueKind != JsonValueKind.Array ||
                routes.GetArrayLength() == 0)
            {
                return null;
            }

            var route = routes[0];
            var polyline = ReadPolyline(route);
            var steps = ReadSteps(route);
            var distance = route.TryGetProperty("distance", out var d) ? d.GetDouble() : 0;
            var duration = route.TryGetProperty("duration", out var t) ? t.GetDouble() : 0;

            return new RouteResult(polyline, distance, duration, steps);
        }
        catch
        {
            return null;
        }
    }

    private static IReadOnlyList<GeoPoint> ReadPolyline(JsonElement route)
    {
        var points = new List<GeoPoint>();
        if (route.TryGetProperty("geometry", out var geom) &&
            geom.TryGetProperty("coordinates", out var coords) &&
            coords.ValueKind == JsonValueKind.Array)
        {
            foreach (var pair in coords.EnumerateArray())
            {
                if (pair.ValueKind == JsonValueKind.Array && pair.GetArrayLength() >= 2)
                {
                    var lon = pair[0].GetDouble();
                    var lat = pair[1].GetDouble();
                    points.Add(new GeoPoint(lat, lon));
                }
            }
        }
        return points;
    }

    private static IReadOnlyList<RouteStep> ReadSteps(JsonElement route)
    {
        var steps = new List<RouteStep>();
        if (!route.TryGetProperty("legs", out var legs) || legs.ValueKind != JsonValueKind.Array)
        {
            return steps;
        }

        foreach (var leg in legs.EnumerateArray())
        {
            if (!leg.TryGetProperty("steps", out var legSteps)) continue;

            foreach (var step in legSteps.EnumerateArray())
            {
                var distance = step.TryGetProperty("distance", out var d) ? d.GetDouble() : 0;
                var instruction = BuildInstruction(step);
                var location = ReadStepLocation(step);
                steps.Add(new RouteStep(instruction, distance, location));
            }
        }

        return steps;
    }

    private static string BuildInstruction(JsonElement step)
    {
        if (!step.TryGetProperty("maneuver", out var maneuver)) return "Continuer";

        var type = maneuver.TryGetProperty("type", out var t) ? t.GetString() : null;
        var modifier = maneuver.TryGetProperty("modifier", out var m) ? m.GetString() : null;
        var name = step.TryGetProperty("name", out var n) ? n.GetString() : null;

        var action = type switch
        {
            "turn" => modifier switch
            {
                "left" => "Tournez à gauche",
                "right" => "Tournez à droite",
                "slight left" => "Légèrement à gauche",
                "slight right" => "Légèrement à droite",
                "sharp left" => "Tournez fortement à gauche",
                "sharp right" => "Tournez fortement à droite",
                _ => "Tournez"
            },
            "depart" => "Démarrez",
            "arrive" => "Vous êtes arrivé",
            "merge" => "Insérez-vous",
            "roundabout" => "Prenez le rond-point",
            "fork" => "Restez sur la voie",
            "end of road" => "Au bout de la rue",
            _ => "Continuez"
        };

        return string.IsNullOrWhiteSpace(name) ? action : $"{action} sur {name}";
    }

    private static GeoPoint ReadStepLocation(JsonElement step)
    {
        if (step.TryGetProperty("maneuver", out var maneuver) &&
            maneuver.TryGetProperty("location", out var loc) &&
            loc.ValueKind == JsonValueKind.Array &&
            loc.GetArrayLength() >= 2)
        {
            return new GeoPoint(loc[1].GetDouble(), loc[0].GetDouble());
        }
        return default;
    }
}
