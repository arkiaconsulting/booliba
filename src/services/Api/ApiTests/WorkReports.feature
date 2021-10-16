Feature: Easily manage and send work reports

Manage work reports

Scenario: Add a report
	Given I have prepared my report for the current month
	When I add my work report
	Then I can see my work report into the work report list
