using System;
using System.Threading.Tasks;
using Acceptance.Tests.PageObjects;
using FluentAssertions;
using TechTalk.SpecFlow;
using Test.Utils;

namespace Acceptance.Tests.Steps;

[Binding]
public class StudentCreationStep
{
    private const string FIRST_NAME = "Federico";
    private const string LAST_NAME = "Teotini";
    private static readonly DateTime ENROLLMENT_DATE = new(2022, 05, 11, 11, 45, 0);
    private readonly CreateStudentPage _page;

    public StudentCreationStep(CreateStudentPage page)
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
        await _page.FillFirstName(FIRST_NAME);
        await _page.FillLastName(LAST_NAME);
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
        await tableRows.Should().ContainSingleMatching(
            async row =>
            {
                (await row.Locator("td").AllInnerTextsAsync()).Should()
                                                              .NotBeEmpty().And
                                                              .Contain(FIRST_NAME).And
                                                              .Contain(LAST_NAME).And
                                                              .Contain(ENROLLMENT_DATE.ToString("MM/dd/yyyy HH:mm:ss"));
            }
        );
    }

    [When(@"an enrolment date")]
    public async Task WhenAnEnrolmentDate()
    {
        await _page.FillEnrollmentDate(ENROLLMENT_DATE);
    }

    [Then(@"the registration form should expose a validation error")]
    public async Task ThenTheRegistrationFormShouldExposeAValidationError()
    {
        _page.CurrentUrl.Should().EndWith(_page.PagePath);
        (await _page.GetValidationErrors()).Should().ContainMatch("*EnrollmentDate*required*");
    }
}