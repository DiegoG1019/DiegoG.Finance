using Blazored.LocalStorage;
using DiegoG.Finance.Blazor.ModelControls;
using DiegoG.Finance.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DiegoG.Finance.Blazor;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddCascadingValue(sp => new ContextProvider());
        builder.Services.AddCascadingValue(sp => new WorkTable());
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<WorkSheetStorage>();
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
#else
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif
        await builder.Build().RunAsync();
    }
}
