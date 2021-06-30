using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CoreApp.Helpers
{
    public class Utilities
    {
        public static async Task<byte[]> GetBinaryResource(string resourceName)
        {
            var assembly = typeof(CoreApp.Controllers.Api.BaseAuthorizedAPIController).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"BullsBluffCore.Resources.{resourceName}");
            using (var ms = new MemoryStream())
            {
                resourceStream.Position = 0;
                await resourceStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public static async Task<string> GetDefaultDogImageThumbnailString()
        {
            return GetThumbnailBase64String(await GetBinaryResource("dogFrontPlaceholder.png"), "dogFrontPlaceholder.png");
        }
        public static string GetThumbnailBase64String(byte[] data, string fileName)
        {
            using (Image<Rgba32> image = Image.Load(data))
            {
                image.Mutate(x => x
                     .Resize(48, 48));
                var format = image.GetConfiguration()
                    .ImageFormatsManager
                    .FindFormatByFileExtension(System.IO.Path.GetExtension(fileName));
                return image.ToBase64String(format);
            }
        }
    }
}