resource "azurerm_app_configuration_key" "cosmosdb_account_endpoint" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  label                  = local.app_config_label
  key                    = "Cosmos:Endpoint"
  value                  = data.azurerm_cosmosdb_account.common.endpoint
}

resource "azurerm_app_configuration_key" "cosmosdb_database_name" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  label                  = local.app_config_label
  key                    = "Cosmos:Database"
  value                  = azurerm_cosmosdb_sql_database.main.name
}

resource "azurerm_app_configuration_key" "cosmosdb_event_store_container_name" {
  configuration_store_id = data.azurerm_app_configuration.common.id
  label                  = local.app_config_label
  key                    = "Cosmos:EventStore"
  value                  = azurerm_cosmosdb_sql_container.event_store.name
}
