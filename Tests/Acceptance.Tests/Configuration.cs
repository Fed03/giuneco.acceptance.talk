using Microsoft.Extensions.Configuration;

namespace Acceptance.Tests;

internal static class Configuration
{
    public static IConfiguration Load() => new ConfigurationBuilder()
                                           .AddJsonFile("appsettings.json", false, true)
                                           .AddJsonFile("appsettings.local.json", true, true)
                                           .Build();
}