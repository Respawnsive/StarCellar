using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.Without.Apizr.Services.Navigation;
using StarCellar.Without.Apizr.Views;

namespace StarCellar.Without.Apizr.ViewModels;

public partial class CellarViewModel : BaseViewModel
{
    private readonly ICellarApi _cellarApi;
    private readonly IConnectivity _connectivity;

    public CellarViewModel(INavigationService navigationService, 
        ICellarApi cellarApi, 
        IConnectivity connectivity) : base(navigationService)
    {
        _cellarApi = cellarApi;
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
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await NavigationService.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            var wines = await _cellarApi.GetWinesAsync();

            if(Wines.Count != 0)
                Wines.Clear();

            foreach(var wine in wines)
                Wines.Add(wine);
        }
        catch (ApiException ex)
        {
            Debug.WriteLine($"Unable to get Wines: {ex.Message}");
            await NavigationService.DisplayAlert($"Error from ex {ex.StatusCode}!", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get Wines: {ex.Message}");
            await NavigationService.DisplayAlert("Error!", ex.Message, "OK");
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

        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await NavigationService.DisplayAlert("No connectivity!",
                $"Please check internet and try again.", "OK");
            return;
        }

        IsBusy = true;

        var wineDetailsResponse = await _cellarApi.GetWineDetailsAsync(wine.Id);

        IsBusy = false;

        if (!wineDetailsResponse.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Unable to get wine details: {wineDetailsResponse.Error!.Message}");
            await NavigationService.DisplayAlert($"Error from rsp {wineDetailsResponse.StatusCode}!", wineDetailsResponse.Error!.Message, "OK");

            return;
        }

        await NavigationService.GoToAsync($"{nameof(WineDetailsPage)}", true, new Dictionary<string, object>
        {
            {nameof(Wine), wine }
        });
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await GetWinesAsync();
    }
}
