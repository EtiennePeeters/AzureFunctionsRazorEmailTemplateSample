using AzureFunctionsRazorEmailTemplateSample;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.ObjectPool;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureFunctionsRazorEmailTemplateSample
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var compiledViewAssembly = Assembly.LoadFile(Path.Combine(executionPath, "AzureFunctionsRazorEmailTemplateSample.Views.dll"));
            builder.Services
                .AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<RazorViewToStringRenderer>()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
                .AddMvcCore()
                .AddViews()
                .AddRazorViewEngine()
                .AddApplicationPart(compiledViewAssembly);
        }
    }
}
