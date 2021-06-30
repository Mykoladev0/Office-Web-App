using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BullITPDF
{
    public class EmbeddedResource
    {
        public static Stream GetResource(string resourceName)
        {
            var assembly = typeof(BullITPDF.ABKCBuilder).GetTypeInfo().Assembly;
            // var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"BullITPDF.Resources.{resourceName}");
            return resourceStream;
            // using (var reader = new (resourceStream, Encoding.UTF8))
            // {
            //     return await reader.ReadToEndAsync();
            // }
        }
    }
}