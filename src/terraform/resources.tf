resource "azurerm_resource_group" "main" {
  name     = "booliba"
  location = "francecentral"
  tags     = local.default_tags
}

resource "azurerm_application_insights" "main" {
  name                = "booliba"
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  location            = azurerm_resource_group.main.location
  workspace_id        = data.azurerm_log_analytics_workspace.common.id
  tags                = local.default_tags
}

resource "azurerm_app_service" "main" {
  name                = "booliba"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  app_service_plan_id = data.azurerm_app_service_plan.common.id
  https_only          = true

  app_settings = {
    "APP_CONFIG_ENDPOINT" = data.azurerm_app_configuration.common.endpoint
    "APP_CONFIG_LABEL"    = local.app_config_label
    "AZURE_CLIENT_ID"     = azurerm_user_assigned_identity.main.client_id
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.main.id]
  }

  site_config {
    always_on                = true
    http2_enabled            = true
    min_tls_version          = "1.2"
    dotnet_framework_version = "v6.0"
  }

  tags = local.default_tags
}

resource "azurerm_cosmosdb_sql_database" "main" {
  name                = "booliba"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name

  autoscale_settings {
    max_throughput = 4000
  }
}

resource "azurerm_cosmosdb_sql_container" "event_store" {
  name                = "event_store"
  resource_group_name = data.azurerm_cosmosdb_account.common.resource_group_name
  account_name        = data.azurerm_cosmosdb_account.common.name
  database_name       = azurerm_cosmosdb_sql_database.main.name
  partition_key_path  = "/aggregateId"
}
