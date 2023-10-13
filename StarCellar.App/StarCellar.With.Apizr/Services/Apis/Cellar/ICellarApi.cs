using Apizr;
using Refit;
using StarCellar.With.Apizr.Services.Apis.Cellar.Dtos;

namespace StarCellar.With.Apizr.Services.Apis.Cellar
{
    [WebApi("/wines")]
    public interface ICellarApi
    {
        [Get("/")]
        Task<IEnumerable<Wine>> GetWinesAsync();

        [Get("/{id}")]
        Task<Wine> GetWineDetailsAsync(Guid id);

        [Post("/")]
        Task<Wine> CreateWineAsync(Wine item);
        
        [Put("/{id}")]
        Task UpdateWineAsync(Guid id, Wine item);
        
        [Delete("/{id}")]
        Task DeleteWineAsync(Guid id);
    }
}
