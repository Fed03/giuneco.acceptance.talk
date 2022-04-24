using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using Microsoft.Playwright;
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
        /*var tableRows = await page.Locator("table tr").GetAll();
        tableRows.Should().ContainSingleMatching(
            async locator =>
            {
                (await locator.AllInnerTextsAsync()).Should()
                                                    .Contain("Federico").And
                                                    .Contain("Teotini");

            }
        );*/
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 1000
            }
        );
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

    public static AndWhichConstraint<GenericCollectionAssertions<T>, T> ContainSingleMatching<T>(
        this GenericCollectionAssertions<T> ass,
        Func<T, Task> matcher,
        string because = "",
        params object[] becauseArgs
    )
    {
        ArgumentNullException.ThrowIfNull(matcher);

        string expectationPrefix =
            "Expected {context:collection} to contain a single item matching matcher{reason}, ";

        bool success = Execute.Assertion
                              .BecauseOf(because, becauseArgs)
                              .WithExpectation(expectationPrefix)
                              .Given(() => ass.Subject)
                              .ForCondition(subject => subject is not null)
                              .FailWith("but collection is <null>.")
                              .Then
                              .ForCondition(subject => subject.Any())
                              .FailWith("but collection is empty.")
                              .Then
                              .ClearExpectation();

        T[] matches = new T[0];
        if (success)
        {
            ICollection<T> actualItems = ass.Subject as ICollection<T> ?? ass.Subject.ToList();

            (T Item, bool Failed)[] matcherResults;
            string[] failures;
            using (var collScope = new AssertionScope())
            {
                matcherResults = actualItems.Select(
                    (element, index) =>
                    {
                        using var itemScope = new AssertionScope();
                        matcher(element).RunSynchronously();
                        var errors = itemScope.Discard();

                        if (errors.Length > 0)
                        {
                            var failures = string.Join(
                                Environment.NewLine,
                                errors.Select(x => x.IndentLines().TrimEnd('.'))
                            );
                            collScope.AddPreFormattedFailure($"At index {index}:{Environment.NewLine}{failures}");
                        }

                        return (Item: element, Failed: errors.Any());
                    }
                ).ToArray();
                failures = collScope.Discard();
            }

            matches = matcherResults.Where(x => !x.Failed).Select(x => x.Item).ToArray();
            int count = matches.Length;
            switch (count)
            {
                case 0:
                    string failureMessage = Environment.NewLine
                                            + string.Join(Environment.NewLine, failures.Select(x => x.IndentLines()));
                    Execute.Assertion
                           .BecauseOf(because, becauseArgs)
                           .WithExpectation(expectationPrefix + "but there is no one:")
                           .FailWith(failureMessage)
                           .Then
                           .ClearExpectation();
                    break;
                case > 1:
                    Execute.Assertion
                           .BecauseOf(because, becauseArgs)
                           .FailWith(
                               expectationPrefix + "but " + count.ToString(CultureInfo.InvariantCulture) +
                               " such items were found."
                           );
                    break;
                default:
                    break;
            }
        }

        return new AndWhichConstraint<GenericCollectionAssertions<T>, T>(ass, matches);
    }

    public static string IndentLines(this string @this)
    {
        return string.Join(
            Environment.NewLine,
            @this.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => $"\t{x}")
        );
    }
}