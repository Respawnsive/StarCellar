using Apizr;
using Apizr.Configuring;
using Fusillade;
using Refit;

namespace StarCellar.With.Apizr.Services.Apis.Files
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        [Priority(Priority.Background)]
        [Headers("Authorization: Bearer")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
