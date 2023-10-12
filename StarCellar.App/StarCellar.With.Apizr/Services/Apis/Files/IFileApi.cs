using Apizr;
using Refit;

namespace StarCellar.With.Apizr.Services.Apis.Files
{
    [WebApi(Constants.BaseAddress)]
    public interface IFileApi
    {
        [Multipart]
        [Post("/upload")]
        Task<string> UploadAsync([AliasAs("file")] StreamPart stream);
    }
}
