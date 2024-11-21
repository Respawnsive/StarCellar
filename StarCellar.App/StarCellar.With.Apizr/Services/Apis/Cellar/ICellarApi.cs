using Apizr;
using Apizr.Caching;
using Apizr.Caching.Attributes;
using Apizr.Logging.Attributes;
using Apizr.Configuring;
using Apizr.Configuring.Request;
using Apizr.Resiliencing.Attributes;
using Fusillade;
using Refit;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;

[assembly:Log]
namespace StarCellar.With.Apizr.Services.Apis.Cellar
{
    [BaseAddress("/wines")]
    public interface ICellarApi
    {
        [Get("/")]
        [ResiliencePipeline("CustomPipeline")]
        [Cache(CacheMode.GetOrFetch, "00:00:10")]
        [Priority(Priority.UserInitiated)]
        Task<IList<WineDTO>> GetWinesAsync([RequestOptions] IApizrRequestOptions options);

        [Get("/{id}")]
        [Cache(CacheMode.FetchOrGet, "00:00:10")]
        [Priority(Priority.Speculative)]
        Task<IApiResponse<WineDTO>> GetSafeWineDetailsAsync(Guid id, [RequestOptions] IApizrRequestOptions options);

        [Get("/{id}")]
        [Cache(CacheMode.FetchOrGet, "00:00:10")]
        [Priority(Priority.Speculative)]
        Task<WineDTO> GetWineDetailsAsync(Guid id, [RequestOptions] IApizrRequestOptions options);

        [Post("/")]
        [Headers("Authorization: Bearer")]
        Task<WineDTO> CreateWineAsync(WineDTO item, [RequestOptions] IApizrRequestOptions options);
        
        [Put("/{id}")]
        [Headers("Authorization: Bearer")]
        Task UpdateWineAsync(Guid id, WineDTO item, [RequestOptions] IApizrRequestOptions options);
        
        [Delete("/{id}")]
        [Headers("Authorization: Bearer")]
        Task DeleteWineAsync(Guid id, [RequestOptions] IApizrRequestOptions options);

        [Delete("/{id}")]
        //[Headers("Authorization: Bearer")]
        Task DeleteWineAsync(Guid id);
    }
}
