using Messenger.Web.Entities;
using Messenger.Web.Helpers;
using Messenger.Web.Hubs;
using Messenger.Web.Mappings;
using Messenger.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // JSON serialization to UTC date format
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // empty error response body
                    options.SuppressMapClientErrors = true;
                });

            // SignalR
            services.AddSignalR();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(
                            "http://localhost:8080",
                            "http://192.168.0.103:8080",
                            "http://192.168.0.104:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();

                    });
            });

            // settings configuration
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.Configure<AppSettings>(appSettingsSection);

            // JWT
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                    // JWT for SignalR
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/chat-hub")))
                            {
                                // read the token out of the query string
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // custom user ID provider (to be able to access user ID in the ChatHub)
            // https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0#use-claims-to-customize-identity-handling
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // MongoDB
            var mongoClient = new MongoClient(appSettings.ConnectionString);
            var db = mongoClient.GetDatabase(appSettings.DatabaseName);
            var userCollection = db.GetCollection<User>(appSettings.UserCollectionName);
            var chatCollection = db.GetCollection<Chat>(appSettings.ChatCollectionName);

            // MongoDB collections
            services.AddSingleton<IMongoCollection<User>>(userCollection);
            services.AddSingleton<IMongoCollection<Chat>>(chatCollection);

            // logic services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatService, ChatService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // custom error handler
            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();
            app.UseCors();
                
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat-hub");
            });
        }
    }
}
