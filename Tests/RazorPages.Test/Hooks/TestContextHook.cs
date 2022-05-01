using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using RazorPages.Test.PageObjects;
using Respawn;
using TechTalk.SpecFlow;

namespace RazorPages.Test;

[Binding]
public sealed class TestContextHook
{
    private static readonly IConfiguration _configuration = Configuration.Load();
    private static readonly Checkpoint _checkpoint = new();

    private static DockerComposeHelper.ComposedService? _composedService;

    [BeforeTestRun]
    public static void DockerComposeUp()
    {
        _composedService = new DockerComposeHelper(_configuration).Start();
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await _checkpoint.Reset(_configuration.GetConnectionString("SchoolContext"));
    }

    [BeforeScenario("StudentCreation")]
    public async Task BeforeStudentCreationScenario(IObjectContainer container)
    {
        var browserContext = await SetupBrowserContext(container);
        container.RegisterInstanceAs(new CreateStudentPage(browserContext));
    }

    [BeforeScenario("ListEnrolledStudents")]
    public async Task BeforeListEnrolledStudentsScenario(IObjectContainer container)
    {
        var browserContext = await SetupBrowserContext(container);

        container.RegisterInstanceAs(new CreateStudentPage(browserContext));
        container.RegisterInstanceAs(new StudentsPage(browserContext));
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