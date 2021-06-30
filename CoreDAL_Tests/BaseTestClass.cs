using CoreDAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL_Tests
{
    public abstract class BaseTestClass
    {
        protected ABKCOnlineContext GetABKCContext([CallerMemberName]string contextName = "")
        {

            if (string.IsNullOrEmpty(contextName))
            {
                contextName = Guid.NewGuid().ToString();
            }
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            //var options = SqliteInMemory
            //    .CreateOptions<ABKCOnlineContext>();
            DbContextOptions<ABKCOnlineContext> options;
            var builder = new DbContextOptionsBuilder<ABKCOnlineContext>();
            builder.UseInMemoryDatabase(contextName).UseInternalServiceProvider(serviceProvider);
            options = builder.Options;
            return new ABKCOnlineContext(options);
        }

        public static async Task<byte[]> GetBinaryResource(string resourceName)
        {
            var assembly = typeof(CoreDAL_Tests.BaseTestClass).GetTypeInfo().Assembly;
            var resources = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"CoreDAL_Tests.Resources.{resourceName}");
            using (var ms = new MemoryStream())
            {
                resourceStream.Position = 0;
                await resourceStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
