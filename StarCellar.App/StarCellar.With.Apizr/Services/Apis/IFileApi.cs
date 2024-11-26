
using Refit;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Apizr.Configuring.Request;
using System.Threading.Tasks;

#nullable enable annotations

namespace StarCellar.Services.Apis
{
    [System.CodeDom.Compiler.GeneratedCode("Refitter", "1.4.0.0")]
    public partial interface IFileApi
    {
        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Multipart]
        [Headers("Accept: application/json")]
        [Post("/upload")]
        Task<string> UploadAsync(StreamPart file, [RequestOptions] IApizrRequestOptions options);
    }

}