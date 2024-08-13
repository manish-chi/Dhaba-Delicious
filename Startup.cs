// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0


using Daba_Delicious.Bots;
using Daba_Delicious.Recognizer;
using Dhaba_Delicious.Middlewares;
using Dhaba_Delicious.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace Daba_Delicious
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(env.ContentRootPath)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false)
                          .AddEnvironmentVariables();


            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;

            });


           

            services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings"));

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //var storage = new MemoryStorage();
            var storage = new BlobsStorage(Configuration["StorageConnectionString"], "bot-state");

            var userState = new UserState(storage);
            services.AddSingleton(userState);

            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            services.AddSingleton<SpellCheckMiddleware>();

            services.AddSingleton<DDRecognizer>();

            // Create a global hashset for our ConversationReferences
            services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DhabaDeliciousBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
