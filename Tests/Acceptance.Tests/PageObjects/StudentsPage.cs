using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Acceptance.Tests.PageObjects;

public class StudentsPage : BasePageObject
{
    public StudentsPage(IBrowserContext browser) : base(browser)
    {
    }

    public override string PagePath => "Students";

    public ILocator GetTable() => Page.Locator("table");

    public Task<string> GetMessage() => Page.Locator(".table-wrapper").InnerTextAsync();
}