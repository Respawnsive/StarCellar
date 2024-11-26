using System.Net;
using System.Reflection;
using Apizr;
using CommunityToolkit.Maui;
using Fusillade;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;
using StarCellar.Services.Apis;
using StarCellar.With.Apizr.Helpers;
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

        builder.Services.ConfigureStarCellarApizrManagers(options => options
            .WithConnectivityHandler<IConnectivity>(connectivity => connectivity.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
            .WithExCatching(OnException)
            .WithAuthenticationHandler(typeof(AuthenticationHandler<>))
            .WithProgress());

        // Apizr
        //builder.Services.AddApizr(
        //    registry => registry
        //        .AddManagerFor<ICellarApi>()
        //        //.AddManagerFor<IFileApi>()
        //        .AddManagerFor<IUserApi>()
        //        .AddUploadManagerWith<string>(options => options
        //            .WithLogging()
        //            .WithBasePath("/upload")
        //            .WithHeaders(["Authorization: Bearer"])
        //            .WithPriority(Priority.Background)),

        //    options => options
        //        .WithBaseAddress(
        //            sp => sp
        //                .GetRequiredService<IConfiguration>()
        //                .GetRequiredSection("AppSettings")
        //                .Get<AppSettings>()
        //                .BaseAddress)
        //        .ConfigureHttpClientBuilder(clientBuilder => clientBuilder
        //            .AddStandardResilienceHandler())
        //        .WithConnectivityHandler<IConnectivity>(connectivity => connectivity.NetworkAccess == Microsoft.Maui.Networking.NetworkAccess.Internet)
        //        .WithExCatching(OnException)
        //        .WithInMemoryCacheHandler()
        //        .WithAutoMapperMappingHandler()
        //        .WithPriority()
        //        //.WithAuthenticationHandler(OnGetTokenAsync, OnSetTokenAsync) // Auth with local factory methods
        //        .WithAuthenticationHandler(typeof(AuthenticationHandler<>)) // Auth with resolved open generic handler
        //        .WithMediation()
        //        .WithProgress()
        //        );

        // Register the in-memory cache
        builder.Services.AddMemoryCache();

        // AutoMapper
        builder.Services.AddAutoMapper(assembly);

        // Authentication handler
        builder.Services.AddTransient(typeof(AuthenticationHandler<>));

        // Allow Lazy resolution
        builder.Services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

        // Register MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Presentation
        builder.Services.AddTransient<LoginViewModel>()
            .AddTransient<LoginPage>()
            .AddTransient<RegisterViewModel>()
            .AddTransient<RegisterPage>()
            .AddTransient<ProfileViewModel>()
            .AddTransient<ProfilePage>()
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
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await navigationService.DisplayAlert("No connectivity!", $"Please check internet and try again.", "OK");
                });
                return true; // Handled
            }
            case OperationCanceledException:
            {
                Debug.WriteLine($"Operation cancelled");
                return true; // Handled
            }
            case ApiException {StatusCode: HttpStatusCode.Unauthorized}:
            {
                Debug.WriteLine($"Unauthorized");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await navigationService.ShowToast("Unauthorized!");
                    await navigationService.GoToAsync($"//{nameof(LoginPage)}");
                });
                return true; // Handled
            }
            default:
                return false;
        }
    }

    private static Task<string> OnGetTokenAsync(HttpRequestMessage msg, CancellationToken ct) =>
        SecureStorage.Default.GetAsync(nameof(Tokens.AccessToken));

    private static Task OnSetTokenAsync(HttpRequestMessage msg, string tk, CancellationToken ct) =>
        SecureStorage.Default.SetAsync(nameof(Tokens.AccessToken), tk);

    internal class Lazier<T>(IServiceProvider provider) : Lazy<T>(provider.GetRequiredService<T>);
}
