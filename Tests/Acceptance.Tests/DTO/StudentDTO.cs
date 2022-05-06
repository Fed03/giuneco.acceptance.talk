using System;

namespace Acceptance.Tests.DTO;

internal class StudentDTO
{
    public string FirstName { get; }
    public string LastName { get; }

    public DateTime EnrollmentDate => DateTime.Now;

    public StudentDTO(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}