using AutoMapper;
using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.Without.Apizr.Services.Apis.Files;
using StarCellar.Without.Apizr.Services.Navigation;

namespace StarCellar.Without.Apizr.ViewModels;

[QueryProperty(nameof(Wine), nameof(Wine))]
public partial class WineEditViewModel : BaseViewModel
{
    private readonly ICellarUserInitiatedApi _cellarUserInitiatedApi;
    private readonly IConnectivity _connectivity;
    private readonly IFilePicker _filePicker;
    private readonly IFileBackgroundApi _fileBackgroundApi;
    private readonly IMapper _mapper;

    public WineEditViewModel(INavigationService navigationService,
        ICellarUserInitiatedApi cellarUserInitiatedApi,
        IConnectivity connectivity,
        IFilePicker filePicker,
        IFileBackgroundApi fileBackgroundApi, 
        IMapper mapper) : base(navigationService)
    {
        _cellarUserInitiatedApi = cellarUserInitiatedApi;
        _connectivity = connectivity;
        _filePicker = filePicker;
        _fileBackgroundApi = fileBackgroundApi;
        _mapper = mapper;
    }

    [ObservableProperty] private Wine _wine;

    [RelayCommand]
    private async Task SetImageAsync()
    {
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

                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await NavigationService.DisplayAlert("No connectivity!",
                        $"Please check internet and try again.", "OK");
                    return;
                }

                // Upload in background as we don't want to block the UI
                await using var stream = await result.OpenReadAsync();
                var streamPart = new StreamPart(stream, result.FileName);
                Wine.ImageUrl = await _fileBackgroundApi.UploadAsync(streamPart);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to set an image: {ex.Message}");
            await NavigationService.DisplayAlert("Error!", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if(string.IsNullOrWhiteSpace(Wine.Name))
            {
                await NavigationService.DisplayAlert("Name required!",
                    $"Please give it a name and try again.", "OK");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await NavigationService.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            IsBusy = true;

            var wineDto = _mapper.Map<WineDTO>(Wine);
            if (wineDto.Id == Guid.Empty)
            {
                wineDto = await _cellarUserInitiatedApi.CreateWineAsync(wineDto);
                Wine = _mapper.Map<Wine>(wineDto);
            }
            else
                await _cellarUserInitiatedApi.UpdateWineAsync(wineDto.Id, wineDto);

            await NavigationService.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to save Wine: {ex.Message}");
            await NavigationService.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
