using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace RazorPages.Test.PageObjects;

public class StudentCreatePage : BasePageObject
{
    public StudentCreatePage(IBrowserContext browser) : base(browser)
    {
    }

    protected override string PagePath => "Students/Create";

    public async Task FillFirstName(string name) => await Page.FillAsync("#Student_FirstMidName", name);
    public async Task FillLastName(string name) => await Page.FillAsync("#Student_LastName", name);

    public async Task FillEnrollmentDate(DateTime date) =>
        await Page.FillAsync("#Student_EnrollmentDate", date.ToString("s"));

    public async Task SubmitRegistration() => await Page.ClickAsync("input[type=submit]");

    public string CurrentUrl => Page.Url;
    public async Task<IReadOnlyList<ILocator>> GetTableRows() => await Page.Locator("table tr").GetAll();
}