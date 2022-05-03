using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Acceptance.Tests.Steps;

[Binding]
public class ListEnrolledStudentsStep
{
    [Given(@"the following enrolled students")]
    public async Task GivenTheFollowingEnrolledStudents(Table table)
    {
        ScenarioContext.StepIsPending();
    }

    [When(@"visiting the students main page")]
    public async Task WhenVisitingTheStudentsMainPage()
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"only the forementioned students should be listed")]
    public async Task ThenOnlyTheForementionedStudentsShouldBeListed()
    {
        ScenarioContext.StepIsPending();
    }
}