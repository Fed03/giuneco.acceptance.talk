@Acceptance
@ListEnrolledStudents
Feature: List Enrolled Students

Scenario: Viewing all enrolled students
	Given the following enrolled students
	| FirstName | LastName  |
	| Bilbo     | Baggins   |
	| Tom       | Bombadill |
	When visiting the students main page
	Then only the forementioned students should be listed
	
Scenario: No enrolled students
	Given there are no enrolled student yet
	When visiting the students main page
	Then the table is hidden
	And a simple message is shown
	
