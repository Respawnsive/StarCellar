
using Refit;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Apizr.Configuring.Request;
using System.Threading.Tasks;

#nullable enable annotations

namespace StarCellar.Services.Apis
{
    [System.CodeDom.Compiler.GeneratedCode("Refitter", "1.4.0.0")]
    public partial interface ICellarApi
    {
        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Get("/wines")]
        Task<ICollection<WineDTO>> GetWinesAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/wines")]
        Task<WineDTO> CreateWineAsync([Body] WineDTO? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/wines")]
        Task<WineDTO> CreateWineAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Get("/wines/{id}")]
        Task<WineDTO> GetWineDetailsAsync(System.Guid id, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>A <see cref="Task"/> that completes when the request is finished.</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Put("/wines/{id}")]
        Task UpdateWineAsync(System.Guid id, [Body] WineDTO? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>A <see cref="Task"/> that completes when the request is finished.</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Put("/wines/{id}")]
        Task UpdateWineAsync(System.Guid id, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>A <see cref="Task"/> that completes when the request is finished.</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Delete("/wines/{id}")]
        Task DeleteWineAsync(System.Guid id, [RequestOptions] IApizrRequestOptions options);
    }

}