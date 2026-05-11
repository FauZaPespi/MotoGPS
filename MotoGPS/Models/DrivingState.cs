namespace MotoGPS.Models;

public sealed record DrivingState(
    int CurrentSpeedKmh,
    int SpeedLimitKmh,
    bool IsNearRadar,
    double? ClosestRadarDistanceM,
    GeoPoint? Position,
    double HeadingDegrees,
    IReadOnlyList<GeoPoint>? RadarPositions = null)
{
    public bool IsSpeeding => CurrentSpeedKmh > SpeedLimitKmh;

    public AlertLevel Level => (IsSpeeding, IsNearRadar) switch
    {
        (true, true)   => AlertLevel.Critical,
        (true, false)  => AlertLevel.Speeding,
        (false, true)  => AlertLevel.Radar,
        _              => AlertLevel.None
    };

    public static DrivingState Empty { get; } = new(
        CurrentSpeedKmh: 0,
        SpeedLimitKmh: 50,
        IsNearRadar: false,
        ClosestRadarDistanceM: null,
        Position: null,
        HeadingDegrees: 0,
        RadarPositions: null);
}
