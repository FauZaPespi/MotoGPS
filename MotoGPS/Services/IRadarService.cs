using MotoGPS.Models;

namespace MotoGPS.Services;

public interface IRadarService
{
    Task<RadarProximity> GetClosestAsync(GeoPoint point, CancellationToken ct = default);
}

public readonly record struct RadarProximity(double? DistanceM, IReadOnlyList<GeoPoint> Positions)
{
    public bool IsWithin500m => DistanceM is { } d && d <= 500;
}
