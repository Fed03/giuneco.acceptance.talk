using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using RazorPages.Test.PageObjects;
using TechTalk.SpecFlow;

namespace RazorPages.Test;

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
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 2000
        });
        var pageObj = new StudentCreatePage(
            await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    BaseURL = _configuration["BaseAddress"]
                }
            )
        );
        
        container.RegisterInstanceAs(playwright);
        container.RegisterInstanceAs(browser);
        container.RegisterInstanceAs(pageObj);
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
}