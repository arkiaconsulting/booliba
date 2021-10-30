Feature: Easily manage and send work reports

Manage work reports

Scenario: Add a report
	Given I have prepared my report for the current month
	When I add my work report
	Then I can see my work report into the work report list

Scenario: Add days to an existing report
	Given I have added a work report
	When I add '2021/12/31' to the work report
	Then I can see that the day '2021/12/31' has been added to the days list

Scenario: Remove days an existing report
	Given I have added a work report
	When I remove one of the days from the work report
	Then I can see that my work report contains one day less

Scenario: Remove a work report
	Given I have added a work report
	When I remove my work report
	Then I can see that my work report is no longer in the work report list

Scenario: Send my work report
	Given I have added a work report
	When I send my report to the following recipients
		| email            |
		| toto@contoso.com |
		| titi@contoso.com |
	Then I can see that my work report has been sent to the following recipients
		| email            |
		| toto@contoso.com |
		| titi@contoso.com |