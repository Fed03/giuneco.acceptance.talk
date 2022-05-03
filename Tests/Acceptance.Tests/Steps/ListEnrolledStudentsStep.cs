using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acceptance.Tests.DTO;
using Acceptance.Tests.PageObjects;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Utils;

namespace Acceptance.Tests.Steps;

[Binding]
public class ListEnrolledStudentsStep
{
    private const string EnrolledStudents = "EnrolledStudents";
    private readonly StudentsPage _studentsPage;
    private readonly CreateStudentPage _createStudentPage;
    private readonly ScenarioContext _scenarioContext;

    public ListEnrolledStudentsStep(StudentsPage studentsPage,
                                    CreateStudentPage createStudentPage,
                                    ScenarioContext scenarioContext
    )
    {
        _studentsPage = studentsPage;
        _createStudentPage = createStudentPage;
        _scenarioContext = scenarioContext;
    }

    [Given(@"the following enrolled students")]
    public async Task GivenTheFollowingEnrolledStudents(Table table)
    {
        var students = table.CreateSet<StudentDTO>().ToList();
        foreach (var student in students)
        {
            await EnrollStudent(student);
        }

        _scenarioContext.Add(EnrolledStudents, students);
    }

    [When(@"visiting the students main page")]
    public async Task WhenVisitingTheStudentsMainPage()
    {
        await _studentsPage.NavigateAsync();
    }

    private async Task EnrollStudent(StudentDTO student)
    {
        await _createStudentPage.NavigateAsync();
        await _createStudentPage.FillFirstName(student.FirstName);
        await _createStudentPage.FillLastName(student.LastName);
        await _createStudentPage.FillEnrollmentDate(student.EnrollmentDate);
        await _createStudentPage.SubmitRegistration();
    }

    [Then(@"only the forementioned students should be listed")]
    public async Task ThenOnlyTheForementionedStudentsShouldBeListed()
    {
        var enrolledStudents = _scenarioContext.Get<IReadOnlyList<StudentDTO>>(EnrolledStudents);
        var rows = await _studentsPage.GetTable().Locator("tr").GetAll();
        rows.Should().HaveCount(enrolledStudents.Count + 1);

        foreach (var student in enrolledStudents)
        {
            await rows.Should().ContainSingleMatching(
                async row =>
                {
                    (await row.Locator("td").AllInnerTextsAsync()).Should()
                                                                  .NotBeEmpty().And
                                                                  .Contain(student.FirstName).And
                                                                  .Contain(student.LastName);
                }
            );
        }
    }

    [Given(@"there are no enrolled student yet")]
    public void GivenThereAreNoEnrolledStudentYet()
    {
    }

    [Then(@"the table is hidden")]
    public async Task ThenTheTableIsHidden()
    {
        (await _studentsPage.GetTable().IsHiddenAsync()).Should().BeTrue();
    }

    [Then(@"a simple message is shown")]
    public async Task ThenASimpleMessageIsShown()
    {
        (await _studentsPage.GetMessage()).Should().Be("No students are enrolled yet!");
    }
}