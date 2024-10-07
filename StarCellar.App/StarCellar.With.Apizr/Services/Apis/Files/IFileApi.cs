using Apizr;
using Apizr.Configuring;
using Refit;

namespace StarCellar.With.Apizr.Services.Apis.Files
{
    [BaseAddress(Constants.BaseAddress)]
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
