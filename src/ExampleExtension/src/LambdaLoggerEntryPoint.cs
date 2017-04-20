using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace ExampleExtension
{
    /// <summary>
    /// A simple lambda function that logs the input stream data.
    /// The handler must be declared as follows:
    /// ExampleExtension::ExampleExtension.LambdaLoggerEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaLoggerEntryPoint
    {
        public async Task FunctionHandlerAsync(Stream requestStream, ILambdaContext lambdaContext)
        {
            string request = "";
            using (var reader = new StreamReader(requestStream))
            {
                request = await reader.ReadToEndAsync();
            }
            lambdaContext.Logger.Log(request);
        }
    }
}