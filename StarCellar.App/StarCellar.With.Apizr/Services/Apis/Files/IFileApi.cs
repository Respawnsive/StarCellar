using Refit;

namespace StarCellar.With.Apizr.Services.Apis.Files
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
