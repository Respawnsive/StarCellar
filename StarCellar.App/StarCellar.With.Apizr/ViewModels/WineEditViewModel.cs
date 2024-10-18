using Apizr;
using Refit;
using StarCellar.With.Apizr.Services.Apis.Cellar;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.With.Apizr.Services.Apis.Files;
using StarCellar.With.Apizr.Services.Navigation;

namespace StarCellar.With.Apizr.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineEditViewModel : BaseViewModel
{
    private readonly IApizrManager<ICellarApi> _cellarApiManager;
    private readonly IConnectivity _connectivity;
    private readonly IFilePicker _filePicker;
    private readonly IApizrManager<IFileApi> _fileApiManager;

    public WineEditViewModel(INavigationService navigationService,
        IApizrManager<ICellarApi> cellarApiManager,
        IConnectivity connectivity,
        IFilePicker filePicker,
        IApizrManager<IFileApi> fileApiManager) : base(navigationService)
    {
        _cellarApiManager = cellarApiManager;
        _connectivity = connectivity;
        _filePicker = filePicker;
        _fileApiManager = fileApiManager;
    }

    [ObservableProperty] private Wine _wine;

    [RelayCommand]
    private async Task SetImageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            var result = await _filePicker.PickAsync();
            if (result != null)
            {
                if (!result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) &&
                    !result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    await NavigationService.DisplayAlert("Format rejected!",
                        $"Please select a jpg or png file only.", "OK");
                    return;
                }

                IsBusy = true;

                await using var stream = await result.OpenReadAsync();
                var streamPart = new StreamPart(stream, result.FileName);
                Wine.ImageUrl = await _fileApiManager.ExecuteAsync(api => api.UploadAsync(streamPart));
            }
        }
        catch (ApizrException ex)
        {
            if (!ex.Handled)
            {
                Debug.WriteLine($"Unable to set an image: {ex.Message}");
                await NavigationService.DisplayAlert("Error!", ex.Message, "OK"); 
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(Wine.Name))
            {
                await NavigationService.DisplayAlert("Name required!",
                    $"Please give it a name and try again.", "OK");
                return;
            }

            IsBusy = true;

            if (Wine.Id == Guid.Empty)
                Wine = await _cellarApiManager.ExecuteAsync<Wine, WineDTO>((opt, api, wineDto) => api.CreateWineAsync(wineDto, opt), Wine);
            else
                await _cellarApiManager.ExecuteAsync<Wine, WineDTO>((opt, api, wineDto) => api.UpdateWineAsync(wineDto.Id, wineDto, opt), Wine);

            await NavigationService.GoToAsync("..");
        }
        catch (ApizrException ex)
        {
            if (!ex.Handled)
            {
                Debug.WriteLine($"Unable to save Wine: {ex.Message}");
                await NavigationService.DisplayAlert("Error!", ex.Message, "OK"); 
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
