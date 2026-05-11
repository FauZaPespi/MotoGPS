namespace MotoGPS.Models;

public sealed record FavoritePlace(string Name, double Latitude, double Longitude)
{
    public GeoPoint AsGeoPoint() => new(Latitude, Longitude);
}
