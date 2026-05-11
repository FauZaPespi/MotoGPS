using System.Text.Json;
using MotoGPS.Models;

namespace MotoGPS.Services;

public sealed class FavoritesService : IFavoritesService
{
    private const string StorageKey = "motogps.favorites";

    private readonly List<FavoritePlace> _places;

    public FavoritesService()
    {
        var raw = Preferences.Default.Get(StorageKey, string.Empty);
        _places = string.IsNullOrEmpty(raw)
            ? new List<FavoritePlace>()
            : (JsonSerializer.Deserialize<List<FavoritePlace>>(raw) ?? new List<FavoritePlace>());
    }

    public IReadOnlyList<FavoritePlace> GetAll() => _places;

    public void Add(FavoritePlace place)
    {
        if (_places.Any(p => p.Name.Equals(place.Name, StringComparison.OrdinalIgnoreCase))) return;
        _places.Add(place);
        Persist();
    }

    public void Remove(FavoritePlace place)
    {
        _places.RemoveAll(p => p.Name.Equals(place.Name, StringComparison.OrdinalIgnoreCase));
        Persist();
    }

    private void Persist() => Preferences.Default.Set(StorageKey, JsonSerializer.Serialize(_places));
}
