Feature: Index
	In order to find my way thru nuserv
	As a user
	I want to see a welcome page

@Browser:AGrade
Scenario: Navigate to welcome page
	Given I navigated to /
	Then the browser title should be "nuserv - private nuget repositories"
	And the title should be "nuserv"
	And the lead should be "private nuget repositories"
