using System.Reflection;
using CommunityToolkit.Maui;
using HttpTracer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Files;
using StarCellar.Without.Apizr.Services.Navigation;
using StarCellar.Without.Apizr.Settings;
using StarCellar.Without.Apizr.ViewModels;
using StarCellar.Without.Apizr.Views;
using UraniumUI;

namespace StarCellar.Without.Apizr;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseUraniumUIBlurs()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialIconFonts();
            });
#if DEBUG
		builder.Logging.AddDebug()
            .SetMinimumLevel(LogLevel.Trace);
#endif
        // Settings
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{typeof(AppSettings).Namespace}.appsettings.json");

        var config = new ConfigurationBuilder()
        .AddJsonStream(stream!)
        .Build();

        builder.Configuration.AddConfiguration(config);

        // Plugins
        builder.Services.AddSingleton(Connectivity.Current)
            .AddSingleton(FilePicker.Default)
            .AddSingleton(SecureStorage.Default)
            .AddSingleton<INavigationService, NavigationService>();

        // Refit
        builder.Services.AddRefitClient<ICellarApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () => new HttpTracerHandler
                {
                    Verbosity = HttpMessageParts.All
                }
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress));

        builder.Services.AddRefitClient<IFileApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () => new HttpTracerHandler
                {
                    Verbosity = HttpMessageParts.All
                }
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress));

        // Presentation
        builder.Services
            .AddSingleton<CellarViewModel>()
            .AddSingleton<CellarPage>()
            .AddTransient<WineDetailsViewModel>()
            .AddTransient<WineDetailsPage>()
            .AddTransient<WineEditViewModel>()
            .AddTransient<WineEditPage>();

        return builder.Build();
	}
}
