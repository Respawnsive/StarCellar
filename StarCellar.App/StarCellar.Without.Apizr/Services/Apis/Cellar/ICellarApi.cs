﻿using Refit;
using StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos;

namespace StarCellar.Without.Apizr.Services.Apis.Cellar
{
    public interface ICellarApi
    {
        [Get("/wines")]
        Task<IEnumerable<Wine>> GetWinesAsync(CancellationToken ct);

        [Get("/wines/{id}")]
        Task<IApiResponse<Wine>> GetWineDetailsAsync(Guid id);

        [Post("/wines")]
        Task<Wine> CreateWineAsync(Wine item);
        
        [Put("/wines/{id}")]
        Task UpdateWineAsync(Guid id, Wine item);
        
        [Delete("/wines/{id}")]
        Task DeleteWineAsync(Guid id);
    }
}
