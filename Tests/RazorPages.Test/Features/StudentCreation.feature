@StudentCreation
Feature: Student Creation

@Acceptance
Scenario: Registering a new Student
	Given some personal information about a new Student
	When inserting them in the registration form
	And submitting them
	Then the new Student should be added to the list of registered ones