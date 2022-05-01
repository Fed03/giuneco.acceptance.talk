using System.Threading.Tasks;
using Microsoft.Playwright;

namespace RazorPages.Test.PageObjects;

public abstract class BasePageObject
{
    private readonly IBrowserContext _browser;

    protected BasePageObject(IBrowserContext browser)
    {
        _browser = browser;
    }

    public async Task NavigateAsync()
    {
        Page = await _browser.NewPageAsync();
        await Page.GotoAsync(PagePath);
    }

    public abstract string PagePath { get; }

    public IPage Page { get; private set; } = null!;
}