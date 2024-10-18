using Apizr;
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
        [Get("/"), ResiliencePipeline("CustomPipeline")]
        Task<IEnumerable<Wine>> GetWinesAsync([RequestOptions] IApizrRequestOptions options);

        [Get("/{id}")]
        Task<IApiResponse<Wine>> GetWineDetailsAsync(Guid id);

        [Post("/")]
        Task<Wine> CreateWineAsync(Wine item);
        
        [Put("/{id}")]
        Task UpdateWineAsync(Guid id, Wine item);
        
        [Delete("/{id}")]
        Task DeleteWineAsync(Guid id);
    }
}
