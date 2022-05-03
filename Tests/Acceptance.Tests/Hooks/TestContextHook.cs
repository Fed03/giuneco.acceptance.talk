using System.Threading.Tasks;
using Acceptance.Tests.PageObjects;
using BoDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
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


    [BeforeScenario("StudentCreation")]
    public async Task BeforeStudentCreationScenario(IObjectContainer container)
    {
        var browserContext = await SetupBrowserContext(container);
        container.RegisterInstanceAs(new CreateStudentPage(browserContext));
    }

    [AfterScenario]
    public async Task AfterScenario(IObjectContainer container)
    {
        await container.Resolve<IBrowser>().DisposeAsync();
        container.Resolve<IPlaywright>().Dispose();
    }

    [AfterTestRun]
    public static void DockerComposeDown()
    {
        _composedService?.Dispose();
    }

    private static async Task<IBrowserContext> SetupBrowserContext(IObjectContainer container)
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync();
        var browserContext = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                BaseURL = _configuration["BaseAddress"]
            }
        );

        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);

        return browserContext;
    }
}