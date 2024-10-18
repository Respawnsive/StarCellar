using Apizr;
using Apizr.Caching;
using Apizr.Caching.Attributes;
using Apizr.Logging.Attributes;
using Apizr.Configuring;
using Apizr.Configuring.Request;
using Apizr.Resiliencing.Attributes;
using Refit;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;

namespace StarCellar.With.Apizr.Services.Apis.Cellar
{
    [BaseAddress("/wines"), Log]
    public interface ICellarApi
    {
        [Get("/"), ResiliencePipeline("CustomPipeline"), Cache(CacheMode.GetOrFetch, "00:00:10")]
        Task<IList<Wine>> GetWinesAsync([RequestOptions] IApizrRequestOptions options);

        [Get("/{id}"), Cache(CacheMode.FetchOrGet, "00:00:10")]
        Task<IApiResponse<Wine>> GetWineDetailsAsync(Guid id);

        [Post("/")]
        Task<Wine> CreateWineAsync(Wine item);
        
        [Put("/{id}")]
        Task UpdateWineAsync(Guid id, Wine item);
        
        [Delete("/{id}")]
        Task DeleteWineAsync(Guid id);
    }
}
