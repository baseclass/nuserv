Feature: Overview
	In order to manage the nuget repositories I've setup 
	As a user
	I want to see an overview of my repositories

@Browser:AGrade
Scenario: Navigate to repository
	Given I navigated to /repository
	Then the browser title should be "nuserv repositories"
	And the title should be "repositories"
	And the lead should be "manage repositories"

@Browser:AGrade
Scenario: Look at list of repositories
	Given I navigated to /repository
	Then I should see atleast 1 existing repository
	And I should see a form to create a new repository

@Browser:AGrade
Scenario: Look at list of repositories details
	Given I navigated to /repository
	Then I should see atleast the following repositories:
	 | Title             | ApiUrl                      | FeedUrl                             | Description                                   |
	 | external/ProjectX | repository/external-projectx | repository/external-projectx/api/v2 | Curated nuget repository for project projectX |