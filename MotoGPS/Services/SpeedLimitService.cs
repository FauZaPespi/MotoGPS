using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class SpeedLimitService : ISpeedLimitService
{
    public Task<int> GetLimitAsync(GeoPoint point, CancellationToken ct = default)
        => Task.FromResult(50);
}
