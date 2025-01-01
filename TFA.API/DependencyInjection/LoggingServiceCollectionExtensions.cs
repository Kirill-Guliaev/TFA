using Serilog;
using Serilog.Filters;

namespace TFA.API.DependencyInjection;

internal static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddApiLoggin(this IServiceCollection serviceDescriptors,
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        return serviceDescriptors.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("Application", "TFA.API")
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.OpenSearch(
                    configuration.GetConnectionString("logs"),
                    indexFormat: "forum-logs-{0:yyyy.MM.dd}"))
            .WriteTo.Logger(lc =>
                lc.Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.Console())
            .CreateLogger()));
    }
}
