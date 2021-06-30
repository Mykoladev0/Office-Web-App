using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.SeedData
{
    public static class HelperClasses
    {
        public static async Task<string> GetTextResource(string resourceName)
        {
            var assembly = typeof(CoreDAL.ABKCOnlineContext).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"CoreDAL.SeedData.Files.{resourceName}");
            // return resourceStream;
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public static async Task<byte[]> GetBinaryResource(string resourceName)
        {
            var assembly = typeof(CoreDAL.ABKCOnlineContext).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"CoreDAL.SeedData.Files.{resourceName}");
            using (var ms = new MemoryStream())
            {
                resourceStream.Position = 0;
                await resourceStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}