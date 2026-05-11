using MotoGPS.Models;

namespace MotoGPS.Services;

public interface ILocationService
{
    event EventHandler<LocationUpdate>? LocationChanged;

    LocationUpdate? Last { get; }

    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
}

public readonly record struct LocationUpdate(
    GeoPoint Point,
    double SpeedKmh,
    double HeadingDegrees,
    DateTimeOffset Timestamp);
