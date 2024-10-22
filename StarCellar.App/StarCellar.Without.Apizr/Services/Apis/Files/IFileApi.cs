using Refit;

namespace StarCellar.Without.Apizr.Services.Apis.Files
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        [Headers("Authorization: Bearer")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }

    public interface IFileBackgroundApi : IFileApi
    {
    }
}
