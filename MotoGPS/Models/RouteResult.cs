namespace MotoGPS.Models;

public sealed record RouteResult(
    IReadOnlyList<GeoPoint> Polyline,
    double TotalDistanceM,
    double TotalDurationSec,
    IReadOnlyList<RouteStep> Steps);

public sealed record RouteStep(
    string Instruction,
    double DistanceM,
    GeoPoint Location);
