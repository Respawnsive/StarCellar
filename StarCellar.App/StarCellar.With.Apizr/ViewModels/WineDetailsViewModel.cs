using Apizr;
using StarCellar.With.Apizr.Services.Apis.Cellar;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.With.Apizr.Services.Navigation;
using StarCellar.With.Apizr.Views;

namespace StarCellar.With.Apizr.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineDetailsViewModel : BaseViewModel
{
    private readonly IApizrManager<ICellarApi> _cellarApiManager;
    private readonly IConnectivity _connectivity;

    public WineDetailsViewModel(INavigationService navigationService, 
        IApizrManager<ICellarApi> cellarApiManager, 
        IConnectivity connectivity) : base(navigationService)
    {
        _cellarApiManager = cellarApiManager;
        _connectivity = connectivity;
    }

    [ObservableProperty]
    Wine _wine;

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        await NavigationService.GoToAsync(nameof(WineEditPage), true, new Dictionary<string, object>
        {
            {nameof(Wine), Wine }
        });
    }

    [RelayCommand]
    private async Task DeleteWineAsync()
    {
        if (IsBusy)
            return;

        try
        {

            var confirm = await NavigationService.DisplayAlert("Delete?",
                $"Please confirm you really want to delete it.", "Confirm", "Cancel");
            if(!confirm)
                return;

            IsBusy = true;

            await _cellarApiManager.ExecuteAsync((opt, api) => api.DeleteWineAsync(Wine.Id, opt));

            await NavigationService.GoToAsync("..");
        }
        catch (ApizrException ex)
        {
            if (!ex.Handled)
            {
                Debug.WriteLine($"Unable to delete Wine: {ex.Message}");
                await NavigationService.DisplayAlert("Error!", ex.Message, "OK"); 
            }
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    private void OnAppearing()
    {
        Wine.ViewCount++;
    }
}
