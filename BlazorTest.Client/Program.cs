using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTest.Client
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                // Add any custom services here
                services.Add(new ServiceDescriptor(
                    typeof(TestRunnerService), 
                    new TestRunnerService(typeof(Program).Assembly)));
            });

            new BrowserRenderer(serviceProvider).AddComponent<App>("app");
        }
    }
}
