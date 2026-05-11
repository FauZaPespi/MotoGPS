using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class DrivingStateService : IDisposable
{
    private readonly ILocationService _location;
    private readonly ISpeedLimitService _speedLimit;
    private readonly IRadarService _radar;

    public DrivingStateService(
        ILocationService location,
        ISpeedLimitService speedLimit,
        IRadarService radar)
    {
        _location = location;
        _speedLimit = speedLimit;
        _radar = radar;

        _location.LocationChanged += OnLocationChanged;
    }

    public event EventHandler<DrivingState>? StateChanged;

    public DrivingState Current { get; private set; } = DrivingState.Empty;

    private async void OnLocationChanged(object? sender, LocationUpdate update)
    {
        try
        {
            var limit = await _speedLimit.GetLimitAsync(update.Point);
            var radar = await _radar.GetClosestAsync(update.Point);

            var newState = new DrivingState(
                CurrentSpeedKmh: (int)Math.Round(update.SpeedKmh),
                SpeedLimitKmh: limit,
                IsNearRadar: radar.IsWithin500m,
                ClosestRadarDistanceM: radar.DistanceM,
                Position: update.Point,
                HeadingDegrees: update.HeadingDegrees,
                RadarPositions: radar.Positions);

            if (newState == Current) return;
            Current = newState;
            StateChanged?.Invoke(this, Current);
        }
        catch
        {
            // Keep previous state on transient lookup failure.
        }
    }

    public void Dispose()
    {
        _location.LocationChanged -= OnLocationChanged;
    }

#if DEBUG
    public void OverrideForDebug(DrivingState state)
    {
        Current = state;
        StateChanged?.Invoke(this, Current);
    }
#endif
}
