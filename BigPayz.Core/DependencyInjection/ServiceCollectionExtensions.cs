using System.Reflection;
using Payment.BigPayz;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBigPayz(
       this IServiceCollection services,
       Action<BigPayzOptions> configureOptions,
       Action<IHttpClientBuilder> ConfigureAiClient)
    {
        ConfigureAiClient?.Invoke(services.AddHttpClient<BigPayzClient>());

        return services
            .AddOptions<BigPayzOptions>()
            .Configure(configureOptions)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddSingleton<BigPayzHasher>();
    }
}
