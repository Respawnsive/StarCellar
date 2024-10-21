using Apizr;
using Apizr.Caching;
using Fusillade;
using StarCellar.With.Apizr.Services.Apis.Cellar;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.With.Apizr.Services.Navigation;
using StarCellar.With.Apizr.Views;

namespace StarCellar.With.Apizr.ViewModels;

public partial class CellarViewModel : BaseViewModel
{
    private readonly IApizrManager<ICellarApi> _cellarApiManager;
    private readonly IConnectivity _connectivity;

    public CellarViewModel(INavigationService navigationService,
        IApizrManager<ICellarApi> cellarApiManager, 
        IConnectivity connectivity) : base(navigationService)
    {
        _cellarApiManager = cellarApiManager;
        _connectivity = connectivity;
    }

    public ObservableCollection<Wine> Wines { get; } = new();

    [ObservableProperty] private bool _isRefreshing;

    [RelayCommand]
    private async Task GetWinesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Wines.Count != 0)
                Wines.Clear();

            var cts = new CancellationTokenSource();
            //cts.CancelAfter(1000); // For cancellation demo only

            var wines = await _cellarApiManager.ExecuteAsync<IList<Wine>, IList<WineDTO>>((opt, api) => api.GetWinesAsync(opt), 
                options => options.WithCancellation(cts.Token));
            
            foreach(var wine in wines)
                Wines.Add(wine);

            // Now fetch details form first wine to anticipate user selection
            IsBusy = false;
            IsRefreshing = false; // We don't want to block UI for speculative fetching

            var firstWine = wines.FirstOrDefault();
            if (firstWine != null)
            {
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await NavigationService.DisplayAlert("No connectivity!",
                        $"Please check internet and try again.", "OK");
                    return;
                }

                await _cellarApiManager.ExecuteAsync((opt, api) => api.GetWineDetailsAsync(firstWine.Id, opt),
                    options => options
                        .WithCaching(CacheMode.FetchOrGet, TimeSpan.FromSeconds(10))
                        .WithPriority(Priority.Speculative));
            }
        }
        catch (ApizrException ex)
        {
            if (!ex.Handled)
            {
                Debug.WriteLine($"Unable to get Wines: {ex.Message}");
                await NavigationService.DisplayAlert("Error!", ex.Message, "OK"); 
            }
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await NavigationService.GoToAsync($"{nameof(WineEditPage)}", true, new Dictionary<string, object>
        {
            {nameof(Wine), new Wine() }
        });
    }

    [RelayCommand]
    private async Task GoToDetailsAsync(Wine wine)
    {
        if (wine == null)
            return;

        IsBusy = true;

        var wineDetailsResponse = await _cellarApiManager.ExecuteAsync(
            (opt, api) => api.GetWineDetailsAsync(wine.Id, opt),
            options => options
                .WithCaching(CacheMode.GetOrFetch, TimeSpan.FromSeconds(10))
                .WithPriority(Priority.UserInitiated));

        IsBusy = false;

        if (!wineDetailsResponse.IsSuccess && // Something went wrong
            !wineDetailsResponse.Exception.Handled) // And it's not yet handled
        {
            Debug.WriteLine($"Unable to get wine details: {wineDetailsResponse.Exception!.Message}");
            await NavigationService.DisplayAlert($"Error from rsp {wineDetailsResponse.ApiResponse.StatusCode}!", wineDetailsResponse.Exception!.Message, "OK"); 
        }

        if (wineDetailsResponse.Result != null) // We got data
        {
            // Toast the data source
            if (wineDetailsResponse.DataSource == ApizrResponseDataSource.Request)
                await NavigationService.ShowToast("Data comes from remote api");
            else if (wineDetailsResponse.DataSource == ApizrResponseDataSource.Cache)
                await NavigationService.ShowToast("Data comes from local cache");

            await NavigationService.GoToAsync($"{nameof(WineDetailsPage)}", true, new Dictionary<string, object>
            {
                {nameof(Wine), wine }
            });
        }
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await GetWinesAsync();
    }
}
