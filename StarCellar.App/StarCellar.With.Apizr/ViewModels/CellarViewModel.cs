﻿using Apizr;
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

            var cts = new CancellationTokenSource();
            //cts.CancelAfter(1000); // For cancellation demo only

            var wines = await _cellarApiManager.ExecuteAsync((opt, api) => api.GetWinesAsync(opt), 
                options => options.WithCancellation(cts.Token));

            if(Wines.Count != 0)
                Wines.Clear();

            foreach(var wine in wines)
                Wines.Add(wine);
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

        var wineDetailsResponse = await _cellarApiManager.ExecuteAsync(api => api.GetWineDetailsAsync(wine.Id));

        IsBusy = false;

        if (!wineDetailsResponse.IsSuccess)
        {
            if (!wineDetailsResponse.Exception.Handled)
            {
                Debug.WriteLine($"Unable to get wine details: {wineDetailsResponse.Exception!.Message}");
                await NavigationService.DisplayAlert($"Error from rsp {wineDetailsResponse.ApiResponse.StatusCode}!", wineDetailsResponse.Exception!.Message, "OK"); 
            }

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
