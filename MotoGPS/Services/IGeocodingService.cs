using MotoGPS.Models;

namespace MotoGPS.Services;

public interface IGeocodingService
{
    Task<IReadOnlyList<GeocodingResult>> SearchAsync(string query, CancellationToken ct = default);
}

public sealed record GeocodingResult(string DisplayName, GeoPoint Point);
