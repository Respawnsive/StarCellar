using AutoMapper;
using Fusillade;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;
using StarCellar.Without.Apizr.Services.Navigation;

namespace StarCellar.Without.Apizr.Handlers
{
    public record GetWineDetailsQuery(Guid Id, Priority Priority) : IRequest<Wine>;

    public class GetWineDetailsHandler : IRequestHandler<GetWineDetailsQuery, Wine>
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly ICellarUserInitiatedApi _cellarUserInitiatedApi;
        private readonly ICellarSpeculativeApi _cellarSpeculativeApi;

        public GetWineDetailsHandler(IMapper mapper, 
            IMemoryCache cache, 
            INavigationService navigationService, 
            IConnectivity connectivity, 
            ICellarUserInitiatedApi cellarUserInitiatedApi, 
            ICellarSpeculativeApi cellarSpeculativeApi)
        {
            _mapper = mapper;
            _cache = cache;
            _navigationService = navigationService;
            _connectivity = connectivity;
            _cellarUserInitiatedApi = cellarUserInitiatedApi;
            _cellarSpeculativeApi = cellarSpeculativeApi;
        }

        /// <inheritdoc />
        public async Task<Wine> Handle(GetWineDetailsQuery request, CancellationToken cancellationToken)
        {
            // FetchOrGet behavior
            Wine wineDetails;
            IApiResponse<WineDTO> wineDetailsResponse = null;

            if(_connectivity.NetworkAccess == NetworkAccess.Internet)
                wineDetailsResponse = request.Priority == Priority.Speculative ?
                    await _cellarSpeculativeApi.GetWineDetailsAsync(request.Id) :
                    await _cellarUserInitiatedApi.GetWineDetailsAsync(request.Id);

            if (wineDetailsResponse?.IsSuccessStatusCode == true)
            {
                // Update cache
                _cache.Set("GetWineDetailsAsync", wineDetailsResponse.Content, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) // Set cache expiration
                });

                await _navigationService.ShowToast("Data fetched from remote api", token: cancellationToken);

                wineDetails = _mapper.Map<Wine>(wineDetailsResponse.Content);
            }
            else if (_cache.TryGetValue("GetWineDetailsAsync", out wineDetails))
            {
                await _navigationService.ShowToast("Data loaded from local cache", token: cancellationToken);
            }

            return wineDetails;
        }
    }
}
