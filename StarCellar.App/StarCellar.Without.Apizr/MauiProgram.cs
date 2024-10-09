using System.Net;
using System.Reflection;
using CommunityToolkit.Maui;
using HttpTracer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
using Polly.Registry;
using Polly.Retry;
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
                .BaseAddress))
            .AddStandardResilienceHandler();
            //.Configure(o =>
            //{
            //    o.CircuitBreaker.MinimumThroughput = 10;
            //})
            //.AddResilienceHandler("myHandler", b =>
            //{
            //    b.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>()
            //        {
            //            FallbackAction = _ => Outcome.FromResultAsValueTask(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
            //        })
            //        .AddConcurrencyLimiter(100)
            //        .AddRetry(new HttpRetryStrategyOptions())
            //        .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions())
            //        .AddTimeout(new HttpTimeoutStrategyOptions());
            //});

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
