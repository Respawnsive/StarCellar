using System.Reflection;
using Apizr;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.With.Apizr.Services.Apis.Cellar;
using StarCellar.With.Apizr.Services.Apis.Files;
using StarCellar.With.Apizr.Services.Navigation;
using StarCellar.With.Apizr.Settings;
using StarCellar.With.Apizr.ViewModels;
using StarCellar.With.Apizr.Views;
using UraniumUI;

namespace StarCellar.With.Apizr;

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

        // Apizr
        builder.Services.AddApizr(
            registry => registry
                .AddManagerFor<ICellarApi>()
                .AddManagerFor<IFileApi>(),

            options => options
                .WithBaseAddress(
                    sp => sp
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
