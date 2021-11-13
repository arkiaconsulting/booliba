Feature: Customers

A short summary of the feature

Scenario: Adding a new customer
	Given I had a contract with a new customer named 'Contoso'
	When I add a new customer
	Then I can see my new customer in the customers list
