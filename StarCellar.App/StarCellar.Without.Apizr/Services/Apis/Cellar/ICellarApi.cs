using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;

namespace StarCellar.Without.Apizr.Services.Apis.Cellar
{
    public interface ICellarApi
    {
        [Get("/wines")]
        Task<IList<WineDTO>> GetWinesAsync(CancellationToken ct);

        [Get("/wines/{id}")]
        Task<IApiResponse<WineDTO>> GetWineDetailsAsync(Guid id);

        [Post("/wines")]
        Task<WineDTO> CreateWineAsync(WineDTO item);
        
        [Put("/wines/{id}")]
        Task UpdateWineAsync(Guid id, WineDTO item);
        
        [Delete("/wines/{id}")]
        Task DeleteWineAsync(Guid id);
    }
}
