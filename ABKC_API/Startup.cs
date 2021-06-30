using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using BullsBluffCore.Mappers.Sieve;
using CoreDAL;
using CoreDAL.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Sinks.ApplicationInsights;
using Sieve.Models;
using Sieve.Services;
using Swashbuckle.AspNetCore.Swagger;

using Okta.AspNetCore;
using Newtonsoft.Json.Serialization;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using CoreApp.Interfaces;
using CoreApp.Services;
using System.Security.Claims;
using CoreApp.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using CoreApp.SignalR;
using Stripe;
using CoreApp.Models;
using CoreDAL.Models;
using SendGrid;

using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ABKCAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Okta serverside authentication
            // OktaMvcOptions oktaMvcOptions = new OktaMvcOptions();
            // Configuration.GetSection("Okta").Bind(oktaMvcOptions);
            // oktaMvcOptions.Scope = new List<string> { "openid", "profile", "email" };
            // oktaMvcOptions.GetClaimsFromUserInfoEndpoint = true;


            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                sharedOptions.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            })
            .AddOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = Configuration["Okta:OktaDomain"],
            });
            // .AddJwtBearer(options =>
            // {
            //     options.Authority = $"{Configuration["Okta:OktaDomain"]}/oauth2/default";//"https://dev-436111.oktapreview.com/oauth2/default";
            //     options.Audience = $"{Configuration["Okta:Audience"]}";//Configuration["Okta:ClientId"];//.ClientId;
            //     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //     {
            //         ClockSkew = TimeSpan.FromMinutes(5),
            //         // Ensure the token hasn't expired:
            //         // RequireExpirationTime = true,
            //         // ValidateLifetime = true,
            //         // // Ensure the token audience matches our audience value (default true):
            //         // ValidateAudience = true,
            //         // ValidAudience = "api://default",
            //         RoleClaimType = ClaimTypes.Role,

            //     };
            //     //     // options.Events = new JwtBearerEvents
            //     //     // {
            //     //     //     OnTokenValidated = context =>
            //     //     //     {
            //     //     //         // Add the access_token as a claim, as we may actually need it
            //     //     //         var accessToken = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            //     //     //         if (accessToken != null)
            //     //     //         {

            //     //     //             ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
            //     //     //             if (identity != null)
            //     //     //             {
            //     //     //                 identity.AddClaim(new Claim("access_token", accessToken.RawData));
            //     //     //             }
            //     //     //         }

            //     //     //         return Task.CompletedTask;
            //     //     //     }
            //     //     // };
            // });


            string[] corsEndpoints = Configuration.GetSection("CorsSafeList").AsEnumerable().ToArray().Select(c => c.Value).ToArray();
            corsEndpoints = corsEndpoints ?? new List<string>().ToArray();
            corsEndpoints = corsEndpoints.Where(c => !string.IsNullOrEmpty(c)).ToArray();
            string localhost1 = "http://localhost:8000";
            if (!corsEndpoints.Contains(localhost1)) corsEndpoints.Append(localhost1);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(corsEndpoints).AllowAnyMethod().AllowAnyHeader()
                        .SetIsOriginAllowed(host => true)//could be hacky
                        .WithExposedHeaders("Authorization", "Accept", "Origin", "Content-Disposition", "Content-Type").AllowCredentials();
                });

                Console.WriteLine("CORS Origins:");
                foreach (var s in options.GetPolicy(MyAllowSpecificOrigins).Origins)
                {
                    Console.WriteLine(s);
                }
            });
            // services.AddRazorPages();
            services.AddAuthorization();
            services.AddControllers();//required for 3.1 api controller routing
                                      // services.AddMvc() //removed for core 3.1
                                      // //.AddJsonOptions(options =>
                                      // // {
                                      // //     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                                      // //     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                      // //     options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                                      // //     options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                                      // // })
                                      //.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

            services.AddSignalR();

            services.AddSpaStaticFiles(c =>
            {
                c.RootPath = "wwwroot";
            });
            services.AddApplicationInsightsTelemetry();

            string connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ABKCOnlineContext>(options =>
                options.UseSqlServer(connString).EnableSensitiveDataLogging(true)
                );
            Console.WriteLine($"Connection String: {connString}");

            AddDALServices(services);
            AddCoreAppServices(services);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ABKC Online API", Version = "v1" });
                // c.DescribeAllEnumsAsStrings();
                // c.DescribeStringEnumsInCamelCase();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. NOTE YOU MUST INCLUDE Bearer before token. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header, //"header",
                    Type = SecuritySchemeType.ApiKey,//"apiKey",
                    Scheme = "Bearer"
                });
                // c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } });
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });



            services.Configure<SieveOptions>(Configuration.GetSection("Sieve"));
            services.Configure<FeesModel>(Configuration.GetSection("RegistrationFees"));

            services.AddScoped<ISieveProcessor, CustomSieveProcessor>();

            services.AddAutoMapper(); // Adding automapper

        }

        internal static void AddDALServices(IServiceCollection services)
        {
            services.AddTransient<IDogService, CoreDAL.Services.DogService>();
            services.AddTransient<IBreedService, CoreDAL.Services.BreedService>();
            services.AddTransient<IOwnerService, CoreDAL.Services.OwnerService>();
            services.AddTransient<ILitterService, CoreDAL.Services.LitterService>();
            services.AddTransient<IShowService, CoreDAL.Services.ShowService>();
            services.AddTransient<IJudgeService, CoreDAL.Services.JudgeService>();
            services.AddTransient<IStyleAndClassService, CoreDAL.Services.StyleAndClassService>();

            services.AddTransient<IGeneralRegistrationService, CoreDAL.Services.GeneralRegistrationService>();
            services.AddTransient<IDogRegistrationService, CoreDAL.Services.DogRegistrationService>();
            services.AddTransient<IJrHandlerService, CoreDAL.Services.JuniorHandlerService>();
            services.AddTransient<IABKCUserService, CoreDAL.Services.UserService>();
            services.AddTransient<IPedigreeService, CoreDAL.Services.PedigreeService>();

            services.AddSingleton<BullITPDF.ABKCBuilder, BullITPDF.ABKCBuilder>();

        }
        internal void AddCoreAppServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpClient<IOktaUserService, UserService>();
            services.AddSingleton<IOktaClient>(new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = Configuration["Okta:OktaDomain"],
                Token = Configuration["Okta:ApiToken"]
            }));

            // use custom OKTA to user claims fetcher
            services.AddSingleton<IClaimsTransformation, OktaClaimsTransformation>();

            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
            // services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));//only needed for razor

            services.AddTransient<ITransactionService, TransactionService>();
            string sendGridKey = Configuration["SendGrid:Key"];
            services.AddSingleton<ISendGridClient, SendGridClient>(x => new SendGridClient(sendGridKey));
            // services.AddHttpClient<ISendGridClient, SendGridClient>();

            //signalr notification services
            services.AddTransient<IRegistrationNotificationService, RegistrationNotificationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ABKCOnlineContext context)
        {

            //setup automatic migrations?
            context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseDatabaseErrorPage();
                // app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                // {
                //     HotModuleReplacement = true,
                //     ReactHotModuleReplacement = true
                // });
            }
            else if (env.IsProduction() || env.IsStaging())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // app.UseStaticFiles();//serve up wwwroot folder
            // This will add "Libs" as another valid static content location
            string staticPath = env.ContentRootPath;
            if (Path.GetDirectoryName(staticPath) != "wwwroot")
            {
                staticPath = Path.Combine(staticPath, "wwwroot");
            }
            //following few lines allows us to serve up the dist folder under wwwroot for static files
            staticPath = Path.Combine(staticPath, "dist");
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(staticPath),
                // FileProvider = new PhysicalFileProvider(
                //     Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist")),
                RequestPath = "/wwwroot"
            });
            app.UseSpaStaticFiles();//core 3.1 add
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // string[] corsEndpoints = Configuration.GetSection("CorsSafeList").AsEnumerable().ToArray().Select(c => c.Value).ToArray();
            // corsEndpoints = corsEndpoints ?? new List<string>().ToArray();
            // corsEndpoints = corsEndpoints.Where(c => !string.IsNullOrEmpty(c)).ToArray();

            // app.UseCors(options => options.WithOrigins(corsEndpoints).AllowAnyMethod().AllowAnyHeader());
            app.UseCors(MyAllowSpecificOrigins);

            //asp core 3.1
            app.UseRouting();
            // app.UseSignalR(routes =>
            // {
            //     routes.MapHub<CoreApp.SignalR.OfficeHub>("/officeHub");
            //     routes.MapHub<CoreApp.SignalR.ConsumerRegistrationHub>("/registrationHub");
            // });
            //end asp core 3.1 changes

            //wire up authentication
            app.UseAuthentication();
            app.UseAuthorization();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABKC Online API");
                // c.RoutePrefix = string.Empty;//make default return the swagger for the app
            });



            if (env.IsDevelopment())
            {
                #region Error handling  
                app.UseExceptionHandler(x =>
                {
                    x.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.ContentType = "application/json";
                        var ex = ctx.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ex, Newtonsoft.Json.Formatting.Indented);
                            await ctx.Response.WriteAsync(json).ConfigureAwait(false);
                            //await ctx.Response.WriteAsync(
                            //    new ErrorModel(ex.Error).ToJson()
                            //    ).ConfigureAwait(false);
                        }
                    });
                });

                #endregion

                //loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                #region Error handling  
                app.UseExceptionHandler(x =>
                {
                    x.Run(async ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        ctx.Response.ContentType = "application/json";
                        await ctx.Response.WriteAsync(string.Empty).ConfigureAwait(false);
                    });
                });
                #endregion

                //loggerFactory.AddDebug(LogLevel.Warning);
            }
            //setup default routes
            //following line is just to open testing for payments
            //             app.UseMvc(routes =>
            // {
            //     routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            // });
            // app.UseMvc();//removed for core 3.1
            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapRazorPages();
                endpoints.MapControllers();//required for API endpoint routing
            });
            // app.Run(async (c) =>
            //     {
            //         c.Response.ContentType = "text/html";
            //         await c.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            //     });

            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
            });

        }

    }
}
