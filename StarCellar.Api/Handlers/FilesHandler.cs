using System.Text;
using StarCellar.Api.Utils;

namespace StarCellar.Api.Handlers
{
    internal static class FilesHandler
    {
        internal static async Task<IResult> UploadAsync(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var uniqueFileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", Guid.NewGuid().ToString().AsSpan(0, 4), Path.GetExtension(fileName));

            var filePath = Path.Combine(Constants.DirectoryPath, uniqueFileName);
            await using var stream = File.OpenWrite(filePath);
            await file.CopyToAsync(stream);

            var fileUriPath = $"{Constants.DirectoryPath}/{uniqueFileName}";

            return Results.Text(fileUriPath, "text/plain", Encoding.UTF8);
        }
    }
}
