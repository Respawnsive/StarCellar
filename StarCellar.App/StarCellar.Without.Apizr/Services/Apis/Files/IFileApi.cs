using Refit;

namespace StarCellar.Without.Apizr.Services.Apis.Files
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
