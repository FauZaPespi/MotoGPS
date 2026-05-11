using MotoGPS.Models;

namespace MotoGPS.Services;

public interface IFavoritesService
{
    IReadOnlyList<FavoritePlace> GetAll();
    void Add(FavoritePlace place);
    void Remove(FavoritePlace place);
}
