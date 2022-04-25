using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Test.Utils;
using Xunit;

namespace RazorPages.Test;

[Trait("Category", "Acceptance")]
public class AddStudentTest : IClassFixture<TestContext>, IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    [Fact]
    public async Task Given_condition()
    {
        var context =
            await _browser.NewContextAsync(new BrowserNewContextOptions { BaseURL = "http://localhost:8080" });
        // Open new page
        var page = await context.NewPageAsync();
        // Go to https://localhost:7014/Students/Create
        await page.GotoAsync("Students/Create");
        // Fill input[name="Student\.LastName"]
        await page.FillAsync("#Student_LastName", "Federico");
        // Fill input[name="Student\.FirstMidName"]
        await page.FillAsync("#Student_FirstMidName", "Teotini");
        // Fill input[name="Student\.EnrollmentDate"]
        await page.FillAsync("#Student_EnrollmentDate", "2019-12-11T10:30");
        // Click input:has-text("Create")
        await page.ClickAsync("input[type=submit]");

        page.Url.Should().EndWith("/Students");

        var tableRows = await page.Locator("table tr").GetAll();
        tableRows.Should().ContainSingleMatching(
            async row =>
            {
                (await row.Locator("td").AllInnerTextsAsync()).Should()
                                                              .Contain("Federico").And
                                                              .Contain("Teotini");
            }
        );
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }
}

public static class ILocatorEx
{
    public static async Task<IReadOnlyList<ILocator>> GetAll(this ILocator loc)
    {
        var count = await loc.CountAsync();
        return Enumerable.Range(0, count)
                         .Select(loc.Nth)
                         .ToList();
    }
}