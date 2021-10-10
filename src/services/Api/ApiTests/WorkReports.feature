Feature: Easily manage and send work reports

Manage work reports

Scenario: Create a report
	Given I have prepared my report for the current month
	When I add my work report
	Then I can see that my work report has been saved
