using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDAL
{
    //https://stackoverflow.com/questions/39006847/how-to-set-entity-framework-core-migration-timeout/39012033
    /// <summary>
    /// used for migrations only, increases the timeout
    /// </summary>
    public class MigrationContextFactory : IDesignTimeDbContextFactory<ABKCOnlineContext>
    {
        public ABKCOnlineContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ABKCOnlineContext>();


            string conString = GetConnectionString();
            optionsBuilder.UseSqlServer(conString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

            return new ABKCOnlineContext(optionsBuilder.Options);
        }


        private string GetConnectionString()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = AppContext.BaseDirectory;

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("DefaultConnection");

            if (String.IsNullOrWhiteSpace(connstr) == true)
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'default'.");
            }
            else
            {
                return connstr;
            }
        }
    }
}
