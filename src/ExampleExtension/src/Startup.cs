using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2;
using ExampleExtension.Accounts;
using ExampleExtension.Events;
using ExampleExtension.Venues;

namespace ExampleExtension
{
    /// <summary>
    /// This class is used to configure the MVC application.
    /// </summary>
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Register the application services for dependency injection.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Extract the extension related settings.
            services.Configure<ExtensionSettings>(settings => {
                var section = Configuration.GetSection("ExtensionSettings");
                section.Bind(settings);
                settings.CipherPassphrase = Environment.GetEnvironmentVariable("CIPHER_PASSPHRASE");
            });

            services.AddMvc();

            // The extension uses DynamoDB for data storage.
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();

            services.AddSingleton<IAccountServices, AccountServices>();
            services.AddSingleton<IEventServices, EventServices>();
            services.AddSingleton<IVenueServices, VenueServices>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLambdaLogger(Configuration.GetLambdaLoggerOptions());
            app.UseMvc();
        }
    }
}
