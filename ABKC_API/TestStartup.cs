using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BullsBluffCore.Mappers.Sieve;
using CoreDAL;
using CoreDAL.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Sieve.Services;

namespace ABKCAPI
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddOpenIdConnect("Auth0", options =>
            {
                //set Auth0 as a domain authority
                //using new String escaping and directly dropping into the configuration object
                options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                //configure the clientId and client secret
                //these keys need to be added to the configuration object (appsettings.json)
                options.ClientId = Configuration["Auth0:ClientId"];
                options.ClientSecret = Configuration["Auth0:ClientSecret"];

                //set response type to code (why?)
                options.ResponseType = "code";

                //configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid");

                // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
                options.CallbackPath = new PathString("/signin-auth0");

                //configure the claims issuer to be Auth0
                options.ClaimsIssuer = "Auth0";

                options.Events = new OpenIdConnectEvents
                {
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    },
                    // //obtain an access token for calling an API
                    // OnRedirectToIdentityProvider = context => {
                    //     context.ProtocolMessage.SetParameter("audience", Configuration["Auth0:ApiIdentifier"]);//YOUR_API_IDENTIFIER?
                    //     return Task.FromResult(0);//need to return something here
                    // }
                };
                //options.Events                   
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration["Auth0:Authority"];
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddMvc()
             .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            //DbContextOptions<ABKCOnlineContext> options;
            //var builder = new DbContextOptionsBuilder<ABKCOnlineContext>();
            //builder.UseInMemoryDatabase<ABKCOnlineContext>("memory");
            //options = builder.Options;

            services.AddDbContext<ABKCOnlineContext>(options =>
            {
                options.UseInMemoryDatabase("memory");
            });

            Startup.AddDALServices(services);
            services.AddScoped<ISieveProcessor, CustomSieveProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //DbContextOptions<ABKCOnlineContext> options;
            //var builder = new DbContextOptionsBuilder<ABKCOnlineContext>();
            //builder.UseInMemoryDatabase<ABKCOnlineContext>("memory");
            //options = builder.Options;
            //var context = new ABKCOnlineContext(options);

            //setup automatic migrations?
            //context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            // app.UseStaticFiles();
            // This will add "Libs" as another valid static content location
            string staticPath = env.ContentRootPath;
            // if (Path.GetDirectoryName(staticPath) != "wwwroot"){
            //     staticPath = Path.Combine(staticPath, "wwwroot");
            // }
            staticPath = Path.Combine(staticPath, "wwwroot", "dist");
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(staticPath),
                // FileProvider = new PhysicalFileProvider(
                //     Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist")),
                RequestPath = "/wwwroot"
            });
            //wire up authentication
            app.UseAuthentication();

            //setup default routes
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Dashboard}/{id?}"
                );
                routes.MapSpaFallbackRoute(
                  name: "spa-fallback",
                  defaults: new { controller = "Home", action = "Dashboard" });
            });
        }
    }
}
