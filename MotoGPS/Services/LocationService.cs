using Microsoft.Maui.Devices.Sensors;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class LocationService : ILocationService, IDisposable
{
    private readonly IGeolocation _geolocation;
    private bool _listening;

    public LocationService(IGeolocation geolocation)
    {
        _geolocation = geolocation;
        _geolocation.LocationChanged += OnDeviceLocationChanged;
    }

    public event EventHandler<LocationUpdate>? LocationChanged;
    public LocationUpdate? Last { get; private set; }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_listening) return;

        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted) return;

        var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
        _listening = await _geolocation.StartListeningForegroundAsync(request);
    }

    public Task StopAsync()
    {
        if (_listening)
        {
            _geolocation.StopListeningForeground();
            _listening = false;
        }
        return Task.CompletedTask;
    }

    private void OnDeviceLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
    {
        var location = e.Location;
        var speedKmh = (location.Speed ?? 0) * 3.6;
        var heading = location.Course ?? 0;

        var update = new LocationUpdate(
            new GeoPoint(location.Latitude, location.Longitude),
            speedKmh,
            heading,
            location.Timestamp);

        Last = update;
        LocationChanged?.Invoke(this, update);
    }

    public void Dispose()
    {
        _geolocation.LocationChanged -= OnDeviceLocationChanged;
        if (_listening)
        {
            _geolocation.StopListeningForeground();
        }
    }

#if DEBUG
    public void SimulateUpdate(GeoPoint point, double speedKmh, double headingDegrees)
    {
        var update = new LocationUpdate(point, speedKmh, headingDegrees, DateTimeOffset.UtcNow);
        Last = update;
        LocationChanged?.Invoke(this, update);
    }
#endif
}
