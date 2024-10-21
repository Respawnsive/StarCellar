﻿using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.Without.Apizr.Services.Navigation;
using StarCellar.Without.Apizr.Views;

namespace StarCellar.Without.Apizr.ViewModels;

public partial class CellarViewModel : BaseViewModel
{
    private readonly ICellarUserInitiatedApi _cellarUserInitiatedApi;
    private readonly ICellarSpeculativeApi _cellarSpeculativeApi;
    private readonly IConnectivity _connectivity;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    private IApiResponse<WineDTO> _wineDetailsResponse;

    public CellarViewModel(INavigationService navigationService, 
        ICellarUserInitiatedApi cellarUserInitiatedApi,
        ICellarSpeculativeApi cellarSpeculativeApi,
        IConnectivity connectivity, 
        IMemoryCache cache, 
        IMapper mapper) : base(navigationService)
    {
        _cellarUserInitiatedApi = cellarUserInitiatedApi;
        _cellarSpeculativeApi = cellarSpeculativeApi;
        _connectivity = connectivity;
        _cache = cache;
        _mapper = mapper;
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
            if (Wines.Count != 0)
                Wines.Clear();

            // GetOrFetch behavior
            if (_cache.TryGetValue("GetWinesAsync", out IList<Wine> wines))
            {
                await NavigationService.ShowToast("Data loaded from local cache");
            }
            else
            {
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await NavigationService.DisplayAlert("No connectivity!",
                        $"Please check internet and try again.", "OK");
                    return;
                }

                IsBusy = true;

                var cts = new CancellationTokenSource();
                //cts.CancelAfter(1000); // For cancellation demo only

                var winesDto = await _cellarUserInitiatedApi.GetWinesAsync(cts.Token);

                // Mapping
                wines = _mapper.Map<IList<Wine>>(winesDto);

                // Update cache
                _cache.Set("GetWinesAsync", wines, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) // Set cache expiration
                });

                await NavigationService.ShowToast("Data fetched from remote api");

                // Now fetch details form first wine to anticipate user selection
                IsBusy = false; // We don't want to block UI for speculative fetching

                var firstWine = wines.FirstOrDefault();
                if(firstWine != null)
                    _wineDetailsResponse = await _cellarSpeculativeApi.GetWineDetailsAsync(firstWine.Id);
            }

            foreach(var wine in wines)
                Wines.Add(wine);
        }
        catch (OperationCanceledException ex)
        {
            // Does not work yet for Android: https://github.com/dotnet/android/issues/5761
            Debug.WriteLine($"Get Wines cancelled: {ex.Message}");
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

        // FetchOrGet behavior
        if ((_wineDetailsResponse?.IsSuccessStatusCode != true || _wineDetailsResponse.Content?.Id != wine.Id) && _connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            IsBusy = true;

            _wineDetailsResponse = await _cellarUserInitiatedApi.GetWineDetailsAsync(wine.Id);

            IsBusy = false;
        }


        if (_wineDetailsResponse?.IsSuccessStatusCode == true)
        {
            // Update cache
            _cache.Set("GetWineDetailsAsync", _wineDetailsResponse.Content, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) // Set cache expiration
            });

            await NavigationService.ShowToast("Data fetched from remote api");

            await NavigationService.GoToAsync($"{nameof(WineDetailsPage)}", true, new Dictionary<string, object>
            {
                {nameof(Wine), wine}
            });
        }
        else
        {
            Debug.WriteLine($"Unable to fetch wine details: {_wineDetailsResponse?.Error!.Message ?? "no network"}");

            if (_cache.TryGetValue("GetWineDetailsAsync", out Wine wineDetails))
            {
                await NavigationService.ShowToast("Data loaded from local cache");

                await NavigationService.GoToAsync($"{nameof(WineDetailsPage)}", true, new Dictionary<string, object>
                {
                    {nameof(Wine), wine}
                });
            }
            else
            {
                await NavigationService.DisplayAlert($"Error: {_wineDetailsResponse?.StatusCode.ToString() ?? "network"} with no cached data!",
                    _wineDetailsResponse?.Error!.Message ?? "no network", "OK"); 
            }
        }
    }

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await GetWinesAsync();
    }
}
