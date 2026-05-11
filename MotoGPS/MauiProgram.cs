using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;
using MotoGPS.Services;

namespace MotoGPS;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddSingleton(_ => new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(8)
        });
        builder.Services.AddSingleton(Geolocation.Default);
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<ISpeedLimitService, SpeedLimitService>();
        builder.Services.AddSingleton<IRadarService, RadarService>();
        builder.Services.AddSingleton<IRoutingService, RoutingService>();
        builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
        builder.Services.AddSingleton<IGeocodingService, GeocodingService>();
        builder.Services.AddSingleton<DrivingStateService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}