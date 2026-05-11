using MotoGPS.Models;

namespace MotoGPS.Services;

public interface ISpeedLimitService
{
    Task<int> GetLimitAsync(GeoPoint point, CancellationToken ct = default);
}
