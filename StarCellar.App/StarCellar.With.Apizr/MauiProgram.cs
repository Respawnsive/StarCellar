using System.Net;
using System.Reflection;
using Apizr;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
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

        // Polly custom pipeline
        builder.Services.AddResiliencePipeline<string, HttpResponseMessage>("CustomPipeline", pipelineBuilder =>
        {
            pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(1));
        });

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
                        .BaseAddress)
                .ConfigureHttpClientBuilder(clientBuilder => clientBuilder
                    .AddStandardResilienceHandler())
                .WithConnectivityHandler<IConnectivity>(connectivity => connectivity.NetworkAccess == NetworkAccess.Internet)
                .WithExCatching(OnException)
                .WithInMemoryCacheHandler()
                .WithAutoMapperMappingHandler());

        // Register the in-memory cache
        builder.Services.AddMemoryCache();

        // AutoMapper
        builder.Services.AddAutoMapper(assembly);

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

    private static async Task<bool> OnException(IServiceProvider serviceProvider, ApizrException ex)
    {
        var navigationService = serviceProvider.GetRequiredService<INavigationService>();
        switch (ex.InnerException)
        {
            case IOException innerEx:
            {
                Debug.WriteLine($"Error: {innerEx.Message}");
                await navigationService.DisplayAlert("No connectivity!", $"Please check internet and try again.", "OK"); 
                return true; // Handled
            }
            case OperationCanceledException:
            {
                Debug.WriteLine($"Operation cancelled");
                return true; // Handled
            }
            default:
                return false;
        }
    }
}
