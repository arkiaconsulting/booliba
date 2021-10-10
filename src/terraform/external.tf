#################### APP CONFIGURATION ####################

data "azurerm_resources" "app_config" {
  type = "Microsoft.AppConfiguration/configurationStores"
  required_tags = {
    "layer" = "common"
  }
}

data "azurehelpers_resource_id" "app_config" {
  resource_id = data.azurerm_resources.app_config.resources.0.id
}

data "azurerm_app_configuration" "common" {
  name                = data.azurerm_resources.app_config.resources.0.name
  resource_group_name = data.azurehelpers_resource_id.app_config.resource_group_name
}

#################### COSMOS DB ACCOUNT ####################

data "azurerm_resources" "cosmosdb_account" {
  type = "Microsoft.DocumentDB/databaseAccounts"
  required_tags = {
    "layer" = "common"
  }
}

data "azurehelpers_resource_id" "cosmosdb_account" {
  resource_id = data.azurerm_resources.cosmosdb_account.resources.0.id
}

data "azurerm_cosmosdb_account" "common" {
  name                = data.azurerm_resources.cosmosdb_account.resources.0.name
  resource_group_name = data.azurehelpers_resource_id.cosmosdb_account.resource_group_name
}

#################### APP SERVICE PLAN ####################

data "azurerm_resources" "app_service_plan" {
  type = "Microsoft.Web/serverFarms"
  required_tags = {
    "layer" = "common"
  }
}

data "azurehelpers_resource_id" "app_service_plan" {
  resource_id = data.azurerm_resources.app_service_plan.resources.0.id
}

data "azurerm_app_service_plan" "common" {
  name                = data.azurerm_resources.app_service_plan.resources.0.name
  resource_group_name = data.azurehelpers_resource_id.app_service_plan.resource_group_name
}

#################### LOG ANALYTICS WORKSPACE ####################

data "azurerm_resources" "log_analytics_workspace" {
  type = "Microsoft.OperationalInsights/workspaces"
  required_tags = {
    "layer" = "common"
  }
}

data "azurehelpers_resource_id" "log_analytics_workspace" {
  resource_id = data.azurerm_resources.log_analytics_workspace.resources.0.id
}

data "azurerm_log_analytics_workspace" "common" {
  name                = data.azurerm_resources.log_analytics_workspace.resources.0.name
  resource_group_name = data.azurehelpers_resource_id.log_analytics_workspace.resource_group_name
}
