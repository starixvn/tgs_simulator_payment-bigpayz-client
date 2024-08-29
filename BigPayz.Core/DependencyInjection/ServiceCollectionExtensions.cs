using System.Reflection;
using Tgs.Payment.U2Pay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddU2Pay(
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
