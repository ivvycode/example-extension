using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ExampleExtension
{
    /// <summary>
    /// This class is the entry point to the MVC lambda function.
    /// The handler must be declared as follows:
    /// ExampleExtension::ExampleExtension.LambdaMvcEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaMvcEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseApiGateway();
        }
    }
}
