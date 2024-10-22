using System.Reflection;
using CommunityToolkit.Maui;
using Fusillade;
using HttpTracer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Files;
using StarCellar.Without.Apizr.Services.Apis.User;
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
        var fileProvider = new EmbeddedFileProvider(assembly);

        var config = new ConfigurationBuilder()
        .AddJsonFile(fileProvider, "appsettings.json", optional: false, reloadOnChange: false)
        .Build();

        builder.Configuration.AddConfiguration(config);

        // Plugins
        builder.Services.AddSingleton(Connectivity.Current)
            .AddSingleton(FilePicker.Default)
            .AddSingleton(SecureStorage.Default)
            .AddSingleton<INavigationService, NavigationService>();

        // Refit
        builder.Services.AddRefitClient<IUserApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                    new HttpTracerHandler(
                        new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.UserInitiated),
                        HttpMessageParts.All)
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress))
            .AddStandardResilienceHandler();

        builder.Services.AddRefitClient<ICellarUserInitiatedApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                    new HttpTracerHandler(
                        new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.UserInitiated),
                        HttpMessageParts.All)
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress))
            .AddStandardResilienceHandler();

        builder.Services.AddRefitClient<ICellarSpeculativeApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                    new HttpTracerHandler(
                        new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Speculative),
                        HttpMessageParts.All)
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress))
            .AddStandardResilienceHandler();

        builder.Services.AddRefitClient<IFileBackgroundApi>(new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                    new HttpTracerHandler(
                        new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Background),
                        HttpMessageParts.RequestHeaders)
            })
            .ConfigureHttpClient((sp, c) => c.BaseAddress = new Uri(sp
                .GetRequiredService<IConfiguration>()
                .GetRequiredSection("AppSettings")
                .Get<AppSettings>()
                .BaseAddress));

        // Register the in-memory cache
        builder.Services.AddMemoryCache();

        // AutoMapper
        builder.Services.AddAutoMapper(assembly);

        // Presentation
        builder.Services.AddTransient<LoginViewModel>()
            .AddTransient<LoginPage>()
            .AddTransient<RegisterViewModel>()
            .AddTransient<RegisterPage>()
            .AddTransient<ProfileViewModel>()
            .AddTransient<ProfilePage>()
            .AddTransient<CellarViewModel>()
            .AddTransient<CellarPage>()
            .AddTransient<WineDetailsViewModel>()
            .AddTransient<WineDetailsPage>()
            .AddTransient<WineEditViewModel>()
            .AddTransient<WineEditPage>();

        return builder.Build();
	}
}
