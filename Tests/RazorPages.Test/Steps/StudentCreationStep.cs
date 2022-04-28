using System.Threading.Tasks;
using FluentAssertions;
using RazorPages.Test.PageObjects;
using TechTalk.SpecFlow;
using Test.Utils;

namespace RazorPages.Test.Steps;

[Binding]
public class StudentCreationStep
{
    private readonly StudentCreatePage _page;

    public StudentCreationStep(StudentCreatePage page)
    {
        _page = page;
    }
    
    [Given(@"some personal information about a new Student")]
    public async Task GivenSomePersonalInformationAboutANewStudent()
    {
        await _page.NavigateAsync();
    }

    [When(@"inserting them in the registration form")]
    public async Task WhenInsertingThemInTheRegistrationForm()
    {
        await _page.FillFirstName("Federico");
        await _page.FillLastName("Teotini");
    }

    [When(@"submitting them")]
    public async Task WhenSubmittingThem()
    {
        await _page.SubmitRegistration();
    }

    [Then(@"the new Student should be added to the list of registered ones")]
    public async Task ThenTheNewStudentShouldBeAddedToTheListOfRegisteredOnes()
    {
        _page.CurrentUrl.Should().EndWith("/Students");
        
        var tableRows = await _page.GetTableRows();
        tableRows.Should().ContainSingleMatching(
            async row =>
            {
                (await row.Locator("td").AllInnerTextsAsync()).Should()
                                                              .Contain("Federico").And
                                                              .Contain("Teotini");
            }
        );
    }
}