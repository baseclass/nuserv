Feature: AddRepository
	In order to add a repository
	As a user
	I want to be able to fill out a simple form with validation.

@Browser:AGrade
Scenario: Successfully add a repository
	Given I navigated to /repository
	And I have entered the following information:
		| Title             | Description                |
		| new repository{0} | New repository Description |
	When I press Save
	Then I should see the following repository:
		 | Title             | ApiUrl                       | FeedUrl                             | Description                |
		 | new repository{0} | repository/new-repository{0} | repository/new-repository{0}/api/v2 | New repository Description |

@Browser:AGrade
Scenario: Add a repository without data
	Given I navigated to /repository
	When I press Save
	Then I should see the error "Name is to short" on name
	And I should see the error "Description is to short" on description

@Browser:AGrade
Scenario Outline: Automatic url generation
	Given I navigated to /repository
	And I have entered the following information:
		| Title   | Description |
		| <Title> |             |
	Then I should see the url "<Expected Url>" 
	Scenarios: 
		| Title          | Expected Url   |
		| New Rep0sitory | new-rep0sitory |
		| group/name     | group-name     |
		| group\\Came    | group-came     |
