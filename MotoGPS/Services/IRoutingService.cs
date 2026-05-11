using MotoGPS.Models;

namespace MotoGPS.Services;

public interface IRoutingService
{
    Task<RouteResult?> CalculateAsync(GeoPoint from, GeoPoint to, CancellationToken ct = default);
}
