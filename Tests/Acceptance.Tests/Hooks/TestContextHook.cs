using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace Acceptance.Tests.Hooks;

[Binding]
public sealed class TestContextHook
{
    private static readonly IConfiguration _configuration = Configuration.Load();

    private static DockerComposeHelper.ComposedService? _composedService;

    [BeforeTestRun]
    public static void DockerComposeUp()
    {
        _composedService = new DockerComposeHelper(_configuration).Start();
    }

    [AfterTestRun]
    public static void DockerComposeDown()
    {
        _composedService?.Dispose();
    }
}