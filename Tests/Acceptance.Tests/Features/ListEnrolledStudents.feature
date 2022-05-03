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
	
