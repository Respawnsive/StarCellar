
using Refit;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Apizr.Configuring.Request;
using System.Threading.Tasks;

#nullable enable annotations

namespace StarCellar.Services.Apis
{
    [System.CodeDom.Compiler.GeneratedCode("Refitter", "1.4.0.0")]
    public partial interface IUserApi
    {
        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/signup")]
        Task<UserDTO> SignUpAsync([Body] SignUpRequest? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/signup")]
        Task<UserDTO> SignUpAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/signin")]
        Task<Tokens> SignInAsync([Body] SignInRequest? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/signin")]
        Task<Tokens> SignInAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/refresh")]
        Task<Tokens> RefreshAsync([Body] Tokens? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Post("/refresh")]
        Task<Tokens> RefreshAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>A <see cref="Task"/> that completes when the request is finished.</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Post("/signout")]
        Task SignOutAsync([Body] Tokens? body, [RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>A <see cref="Task"/> that completes when the request is finished.</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Post("/signout")]
        Task SignOutAsync([RequestOptions] IApizrRequestOptions options);

        /// <param name="options">The <see cref="IApizrRequestOptions"/> instance to pass through the request.</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">Thrown when the request returns a non-success status code.</exception>
        [Headers("Accept: application/json")]
        [Get("/profile")]
        Task<UserDTO> GetProfileAsync([RequestOptions] IApizrRequestOptions options);
    }

}