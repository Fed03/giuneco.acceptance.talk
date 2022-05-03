using TechTalk.SpecFlow;

namespace Acceptance.Tests.Steps;

[Binding]
public class StudentCreationStep
{
    [Given(@"some personal information about a new Student")]
    public void GivenSomePersonalInformationAboutANewStudent()
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"inserting them in the registration form")]
    public void WhenInsertingThemInTheRegistrationForm()
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"an enrolment date")]
    public void WhenAnEnrolmentDate()
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"submitting them")]
    public void WhenSubmittingThem()
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"the new Student should be added to the list of registered ones")]
    public void ThenTheNewStudentShouldBeAddedToTheListOfRegisteredOnes()
    {
        ScenarioContext.StepIsPending();
    }
}