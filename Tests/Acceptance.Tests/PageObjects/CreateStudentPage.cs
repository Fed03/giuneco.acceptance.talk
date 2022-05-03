using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Acceptance.Tests.PageObjects;

public class CreateStudentPage : BasePageObject
{
    public CreateStudentPage(IBrowserContext browser) : base(browser)
    {
    }

    public override string PagePath => "Students/Create";

    public Task FillFirstName(string name) => Page.FillAsync("#Student_FirstMidName", name);
    public Task FillLastName(string name) => Page.FillAsync("#Student_LastName", name);

    public Task FillEnrollmentDate(DateTime date) =>
        Page.FillAsync("#Student_EnrollmentDate", date.ToString("s"));

    public Task SubmitRegistration() => Page.ClickAsync("input[type=submit]");

    public string CurrentUrl => Page.Url;
    public Task<IReadOnlyList<ILocator>> GetTableRows() => Page.Locator("table tr").GetAll();
}