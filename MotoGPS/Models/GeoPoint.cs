namespace MotoGPS.Models;

public readonly record struct GeoPoint(double Latitude, double Longitude)
{
    public double DistanceMetersTo(GeoPoint other)
    {
        const double earthRadiusM = 6_371_000;
        var lat1 = Latitude * Math.PI / 180.0;
        var lat2 = other.Latitude * Math.PI / 180.0;
        var dLat = (other.Latitude - Latitude) * Math.PI / 180.0;
        var dLon = (other.Longitude - Longitude) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusM * c;
    }
}
