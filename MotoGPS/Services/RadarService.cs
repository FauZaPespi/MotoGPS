using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class RadarService : IRadarService
{
    public Task<RadarProximity> GetClosestAsync(GeoPoint point, CancellationToken ct = default)
        => Task.FromResult(new RadarProximity(DistanceM: null));
}
